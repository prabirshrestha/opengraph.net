using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Reflection;
using Facebook.OpenGraph.Metadata;
using System.IO;
using System.Security.Permissions;
using System.Linq.Expressions;

namespace Facebook.Graph.Util
{
    /// <summary>
    /// Walks a JToken tree with the path specified by <paramref name="name"/>.
    /// </summary>
    /// <remarks>This type supports the OpenGraph Graph API infrastructure and is not intended to be used by client code.</remarks>
    /// <param name="source">The source token.</param>
    /// <param name="name">The path to the desired data.</param>
    /// <returns>A JToken at the specified path.</returns>
    public delegate JToken WalkToValueCallback(JToken source, string name);

#pragma warning disable 1591
    internal static class DynamicJsonManager
    {
        #region helper classes
        private class GraphPropertyDescriptor
        {
            public PropertyInfo Property;
            public JsonPropertyAttribute JsonAttribute;
            public string JsonName { get { return JsonAttribute.PropertyName; } }

            public static IEnumerable<GraphPropertyDescriptor> GetPropertiesForType(Type t)
            {
                var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                foreach (var prop in properties)
                {
                    JsonPropertyAttribute attr = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true).OfType<JsonPropertyAttribute>().FirstOrDefault();
                    if (attr == null)
                        continue;

                    yield return new GraphPropertyDescriptor { Property = prop, JsonAttribute = attr };
                }
            }

            public static IComparer<GraphPropertyDescriptor> IdAlwaysFirstComparer = new GraphPropertyDescriptorComparer();
        }

        private class GraphPropertyDescriptorComparer : IComparer<GraphPropertyDescriptor>
        {

            #region IComparer<GraphPropertyDescriptor> Members

            public int Compare(GraphPropertyDescriptor x, GraphPropertyDescriptor y)
            {
                if (x.JsonName == "id" && y.JsonName == "id")
                    return 0;
                if (x.JsonName == "id")
                    return -1;
                if (y.JsonName == "id")
                    return 1;
                return x.JsonName.CompareTo(y.JsonName);
            }

            #endregion
        }
        #endregion

        #region Fields containing reflected types
        private static readonly Type ConnectionType = typeof(IConnection);
        private static readonly Type UnclosedConnectionType = typeof(Connection<>);
        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type NullableDateTimeType = typeof(DateTime?);
        private static readonly Type Int32Type = typeof(int);
        private static readonly Type NullableInt32Type = typeof(int?);
        private static readonly Type StringType = typeof(string);
        private static readonly Type OpenGraphEntityType = typeof(GraphEntity);
        private static readonly Type SingleType = typeof(float);
        private static readonly Type NullableSingleType = typeof(float?);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type NullableDoubleType = typeof(double?);

        private static WalkToValueCallback GlobalWalkToValueCallback;
        private static MethodInfo StringIsNullOrEmpty, CastJTokenToString, CastJTokenToInt32, CastJTokenToSingle, CastJTokenToDouble, WalkToValueCallbackInvoke, DateTimeTryParse, Int32TryParse, ObjectToString;
        private static PropertyInfo JTokenItem, JArrayCount, JArrayItemInt32;
        //private static ConstructorInfo InvalidDataExceptionCtor;

        private static void Initialize()
        {
            ActiveMethods = new Dictionary<Type, PopulatePropertiesCallback>();

            GlobalWalkToValueCallback = WalkToValue;
            StringIsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty", BindingFlags.Static | BindingFlags.Public);
            CastJTokenToString = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(string))).First();
            CastJTokenToInt32 = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(int))).First();
            CastJTokenToSingle = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(float))).First();
            CastJTokenToDouble = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(double))).First();
            WalkToValueCallbackInvoke = typeof(WalkToValueCallback).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
            //InvalidDataExceptionCtor = typeof(InvalidDataException).GetConstructor(new Type[] { typeof(string) });
            DateTimeTryParse = typeof(DateTime).GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(DateTime).MakeByRefType() }, null);
            Int32TryParse = typeof(int).GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(int).MakeByRefType() }, null);
            ObjectToString = typeof(object).GetMethod("ToString");

            JTokenItem = typeof(JToken).GetProperty("Item", typeof(JToken), new Type[] { typeof(object) });
            JArrayCount = typeof(JArray).GetProperty("Count");
            JArrayItemInt32 = typeof(JArray).GetProperty("Item", typeof(JToken), new Type[] { typeof(int) });

            TypeMappedGenerators = new Dictionary<Type, GenerateILCallback>();
            TypeMappedGenerators.Add(typeof(byte), GenerateILForStructProperty<byte>);
            TypeMappedGenerators.Add(typeof(short), GenerateILForStructProperty<short>);
            TypeMappedGenerators.Add(typeof(int), GenerateILForStructProperty<int>);
            TypeMappedGenerators.Add(typeof(long), GenerateILForStructProperty<long>);
            TypeMappedGenerators.Add(typeof(float), GenerateILForStructProperty<float>);
            TypeMappedGenerators.Add(typeof(double), GenerateILForStructProperty<double>);
            TypeMappedGenerators.Add(typeof(byte?), GenerateILForNullableStructProperty<byte>);
            TypeMappedGenerators.Add(typeof(short?), GenerateILForNullableStructProperty<short>);
            TypeMappedGenerators.Add(typeof(int?), GenerateILForNullableStructProperty<int>);
            TypeMappedGenerators.Add(typeof(long?), GenerateILForNullableStructProperty<long>);
            TypeMappedGenerators.Add(typeof(float?), GenerateILForNullableStructProperty<float>);
            TypeMappedGenerators.Add(typeof(double?), GenerateILForNullableStructProperty<double>);
            TypeMappedGenerators.Add(typeof(string), GenerateILForStringProperty);
            TypeMappedGenerators.Add(typeof(DateTime), GenerateILForDateTimeProperty);
            TypeMappedGenerators.Add(typeof(DateTime?), GenerateILForNullableDateTimeProperty);
        }
        #endregion

        #region Fields containing implementation components
        private delegate void PopulatePropertiesCallback(JToken source, GraphEntity objectToPopulate, WalkToValueCallback callback,
            GraphSession session);
        private static Dictionary<Type, PopulatePropertiesCallback> ActiveMethods;
        private delegate void GenerateILCallback(ILGenerator generator, PropertyInfo property, JsonPropertyAttribute attr);
        private static Dictionary<Type, GenerateILCallback> TypeMappedGenerators;

        private static JToken WalkToValue(JToken source, string name)
        {
            if (source == null)
                return null;

            string[] tokens = name.Split('/');

            if (tokens.Length == 0)
                return null;

            JToken current = source[tokens[0]];
            for (int i = 1; i < tokens.Length; i++)
            {
                if (current == null)
                    return null;

                current = current[tokens[i]];
            }

            return current;
        }


        private static GenerateILCallback GetCallback(PropertyInfo property)
        {
            GenerateILCallback callback = null;
            if (!TypeMappedGenerators.TryGetValue(property.PropertyType, out callback))
            {
                if (ConnectionType.IsAssignableFrom(property.PropertyType))
                {
                    TypeMappedGenerators.Add(property.PropertyType, GenerateILForConnectionProperty);
                    callback = GenerateILForConnectionProperty;
                }
                else if (OpenGraphEntityType.IsAssignableFrom(property.PropertyType))
                {
                    TypeMappedGenerators.Add(property.PropertyType, GenerateILForOpenGraphTypeProperty);
                    callback = GenerateILForOpenGraphTypeProperty;
                }
                else if (property.PropertyType.IsArray && OpenGraphEntityType.IsAssignableFrom(property.PropertyType.GetElementType()))
                {
                    TypeMappedGenerators.Add(property.PropertyType, GenerateILForOpenGraphTypeArrayProperty);
                    callback = GenerateILForOpenGraphTypeArrayProperty; ;
                }
            }

            if (callback == null)
                throw new InvalidOperationException("Could not deserialize property '" + property.DeclaringType.FullName + "::" + property.Name + "': no suitable deserialization method for type '" + property.PropertyType.FullName + "' could be found.");

            return callback;
        }
        #endregion

        #region API
        public static void PopulateProperties(JToken source, GraphSession session, GraphEntity objectToPopulate)
        {
            if (source == null)
                return;

            if (source.HasValues)
            {
                JToken error = source["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }

            if (objectToPopulate == null)
                return;

            Type me = objectToPopulate.GetType();
            PopulatePropertiesCallback callback = RetrievePopulateMethod(me);
            callback(source, objectToPopulate, GlobalWalkToValueCallback, session);
            objectToPopulate.Session = session;
        }

        private static PopulatePropertiesCallback RetrievePopulateMethod(Type me)
        {
            if (ActiveMethods == null)
                Initialize();

            PopulatePropertiesCallback result = null;
            if (!ActiveMethods.TryGetValue(me, out result))
            {
                DynamicMethod method = CreateDynamicMethodToPopulate(me);
                result = method.CreateDelegate(typeof(PopulatePropertiesCallback)) as PopulatePropertiesCallback;
                ActiveMethods.Add(me, result);
            }
            return result;
        }
        #endregion

        private static DynamicMethod CreateDynamicMethodToPopulate(Type type)
        {
            if (!OpenGraphEntityType.IsAssignableFrom(type))
                throw new InvalidCastException("Parameter 'type' is not compatible with 'Facebook.Graph.OpenGraphEntity'.");

#if DEBUG && !SILVERLIGHT
            string asmFileName = "GraphApi_Populate_" + Regex.Replace(type.FullName, "\\W+", "_");
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(asmFileName),
                AssemblyBuilderAccess.RunAndSave
                );
            ModuleBuilder mod = ab.DefineDynamicModule(asmFileName + ".dll", asmFileName + ".dll");
            TypeBuilder tb = mod.DefineType("Populate", TypeAttributes.Class | TypeAttributes.Sealed);
            MethodBuilder meth = tb.DefineMethod("Go", MethodAttributes.Static | MethodAttributes.Public, typeof(void),
                new Type[] { typeof(JToken), typeof(GraphEntity), typeof(WalkToValueCallback), typeof(GraphSession) }
                );
#endif

            DynamicMethod method = new DynamicMethod(
                "GraphApi$Populate$" + Regex.Replace(type.FullName, "\\W+", "_"),
                typeof(void),
                new Type[] { typeof(JToken), typeof(GraphEntity), typeof(WalkToValueCallback), typeof(GraphSession) }
                );

            var gen = method.GetILGenerator();
            var typedLocal = gen.DeclareLocal(type);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Castclass, type);
            gen.Emit(OpCodes.Stloc_0);
#if DEBUG && !SILVERLIGHT
            var gen2 = meth.GetILGenerator();
            var typedLocal2 = gen2.DeclareLocal(type);
            gen2.Emit(OpCodes.Ldarg_1);
            gen2.Emit(OpCodes.Castclass, type);
            gen2.Emit(OpCodes.Stloc_0);
#endif
            var properties = GraphPropertyDescriptor.GetPropertiesForType(type).OrderBy(g => g, GraphPropertyDescriptor.IdAlwaysFirstComparer);
            foreach (var prop in properties)
            {
                GenerateILCallback cb = GetCallback(prop.Property);
                cb(gen, prop.Property, prop.JsonAttribute);
#if DEBUG && !SILVERLIGHT
                cb(gen2, prop.Property, prop.JsonAttribute);
#endif
            }
            gen.Emit(OpCodes.Ret);

#if DEBUG && !SILVERLIGHT
            gen2.Emit(OpCodes.Ret);
            Type result = tb.CreateType();
            
            ab.Save(asmFileName + ".dll");
#endif

            return method;
        }


        #region GenerateILForX overloads
        private static void GenerateILForStructProperty<T>(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
            where T : struct
        {
            // determine appropriate casting method
            MethodInfo cast = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(T))).FirstOrDefault();
            if (cast == null)
                throw new InvalidCastException("Cannot cast token to property '" + prop.Name + "' of type '" + prop.DeclaringType.FullName + "' because no suitable cast method exists.");

            LocalBuilder local = gen.DeclareLocal(typeof(T));
            LocalBuilder tmpToken = gen.DeclareLocal(typeof(JToken));
            Label wasNull = gen.DefineLabel();

            string name = attr.PropertyName;

            if (name.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, name);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, name);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            if (attr.ShouldDefaultIfNull)
            {
                gen.Emit(OpCodes.Stloc, tmpToken);
                gen.Emit(OpCodes.Ldloc, tmpToken);
                gen.Emit(OpCodes.Ldnull);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.Brtrue_S, wasNull);

                gen.Emit(OpCodes.Ldloc, tmpToken);
            }

            gen.Emit(OpCodes.Call, cast);
            gen.Emit(OpCodes.Stloc, local);

            gen.MarkLabel(wasNull);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());
            gen.Emit(OpCodes.Nop); // property boundary
        }

        private static void GenerateILForNullableStructProperty<T>(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            MethodInfo cast = typeof(JToken).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.Name == "op_Explicit" && mi.ReturnType.Equals(typeof(T))).FirstOrDefault();
            if (cast == null)
                throw new InvalidCastException("Cannot cast token to property '" + prop.Name + "' of type '" + prop.DeclaringType.FullName + "' because no suitable cast method exists.");

            LocalBuilder local = gen.DeclareLocal(typeof(T));
            LocalBuilder tmpToken = gen.DeclareLocal(typeof(JToken));
            Label wasNull = gen.DefineLabel();

            string name = attr.PropertyName;

            if (name.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, name);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, name);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            gen.Emit(OpCodes.Stloc, tmpToken);
            gen.Emit(OpCodes.Ldloc, tmpToken);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, wasNull);

            gen.Emit(OpCodes.Ldloc, tmpToken);
            gen.Emit(OpCodes.Call, cast);
            gen.Emit(OpCodes.Stloc, local);

            gen.MarkLabel(wasNull);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());
            gen.Emit(OpCodes.Nop); // property boundary
        }

        private static void GenerateILForDateTimeProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            string token = attr.PropertyName;
            LocalBuilder tmpStr = gen.DeclareLocal(typeof(string));
            LocalBuilder local = gen.DeclareLocal(typeof(DateTime));

            Label parseSucceeded = gen.DefineLabel();

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }
            gen.Emit(OpCodes.Call, CastJTokenToString);
            gen.Emit(OpCodes.Stloc, tmpStr);

            if (attr.ShouldDefaultIfNull)
            {
                gen.Emit(OpCodes.Ldloc, tmpStr);
                gen.Emit(OpCodes.Ldnull);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.Brtrue_S, parseSucceeded);
            } 

            gen.Emit(OpCodes.Ldloc, tmpStr);
            gen.Emit(OpCodes.Ldloca, local.LocalIndex);
            gen.Emit(OpCodes.Call, DateTimeTryParse);

            gen.Emit(OpCodes.Brtrue_S, parseSucceeded);

            gen.Emit(OpCodes.Ldstr, "Could not convert value of property '" + token + "' to a DateTime.");
            gen.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[] { typeof(string) }));
            gen.Emit(OpCodes.Throw);

            gen.MarkLabel(parseSucceeded);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());
        }

        private static void GenerateILForNullableDateTimeProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            Label notSettingProperty = gen.DefineLabel();
            string token = attr.PropertyName;

            LocalBuilder result = gen.DeclareLocal(typeof(DateTime));
            LocalBuilder tmpTkn = gen.DeclareLocal(typeof(JToken));
            LocalBuilder tmp = gen.DeclareLocal(typeof(string));

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            gen.Emit(OpCodes.Stloc, tmpTkn); // src = Get(source);
            gen.Emit(OpCodes.Ldloc, tmpTkn);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, notSettingProperty); // if (src != null)

            gen.Emit(OpCodes.Ldloc, tmpTkn);
            gen.Emit(OpCodes.Call, CastJTokenToString);
            gen.Emit(OpCodes.Stloc, tmp); // tmp = (string)src;

            gen.Emit(OpCodes.Ldloc, tmp);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, notSettingProperty); // if (tmp != null)

            gen.Emit(OpCodes.Ldloc, tmp);
            gen.Emit(OpCodes.Ldloca_S, result);
            gen.Emit(OpCodes.Call, DateTimeTryParse);
            gen.Emit(OpCodes.Brfalse_S, notSettingProperty); // if (!DateTime.TryParse(tmp, out result)) {

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, result);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());

            gen.MarkLabel(notSettingProperty);
            gen.Emit(OpCodes.Nop);
        }

        private static void GenerateILForOpenGraphTypeProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            Label noVal = gen.DefineLabel();
            string token = attr.PropertyName;

            LocalBuilder result = gen.DeclareLocal(prop.PropertyType);
            LocalBuilder tkn = gen.DeclareLocal(typeof(JToken));

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }
            gen.Emit(OpCodes.Stloc, tkn);

            gen.Emit(OpCodes.Ldloc, tkn);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, noVal);

            gen.Emit(OpCodes.Ldloc, tkn);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Call, typeof(EntityFromTokenFactory<>).MakeGenericType(prop.PropertyType).GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
            gen.Emit(OpCodes.Stloc, result);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, result);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());

            gen.MarkLabel(noVal);
            gen.Emit(OpCodes.Nop);
        }

        private static void GenerateILForConnectionProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            string token = attr.PropertyName;
            LocalBuilder local = gen.DeclareLocal(typeof(JToken));
            LocalBuilder id = gen.DeclareLocal(typeof(string));
            LocalBuilder connection = gen.DeclareLocal(prop.PropertyType);

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, OpenGraphEntityType.GetProperty("ID").GetGetMethod());
            gen.Emit(OpCodes.Stloc, id); // id = entity.ID;

            gen.Emit(OpCodes.Ldloc, id);
            gen.Emit(OpCodes.Ldstr, attr.PropertyName);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Newobj, prop.PropertyType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(string), typeof(string), typeof(JToken), typeof(GraphSession) }, null));
            gen.Emit(OpCodes.Stloc, connection);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, connection);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());

            gen.Emit(OpCodes.Nop); // exit
        }

        private static void GenerateILForOpenGraphTypeArrayProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            LocalBuilder tmpToken = gen.DeclareLocal(typeof(JToken));
            LocalBuilder tmpArray = gen.DeclareLocal(typeof(JArray));
            LocalBuilder result = gen.DeclareLocal(prop.PropertyType);
            LocalBuilder count = gen.DeclareLocal(typeof(int));
            LocalBuilder accumulator = gen.DeclareLocal(typeof(int));
            Label notSetting = gen.DefineLabel();
            Label checkForTermination = gen.DefineLabel();
            Label doWork = gen.DefineLabel();

            string token = attr.PropertyName;

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            gen.Emit(OpCodes.Stloc, tmpToken);
            gen.Emit(OpCodes.Ldloc, tmpToken);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue, notSetting);

            gen.Emit(OpCodes.Ldloc, tmpToken);
            gen.Emit(OpCodes.Castclass, typeof(JArray));
            gen.Emit(OpCodes.Stloc, tmpArray);
            gen.Emit(OpCodes.Ldloc, tmpArray);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue, notSetting);

            gen.Emit(OpCodes.Ldloc, tmpArray);
            gen.Emit(OpCodes.Callvirt, JArrayCount.GetGetMethod());
            gen.Emit(OpCodes.Stloc, count);

            gen.Emit(OpCodes.Ldloc, count);
            gen.Emit(OpCodes.Newarr, prop.PropertyType.GetElementType());
            gen.Emit(OpCodes.Stloc, result);

            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Stloc, accumulator);
            gen.Emit(OpCodes.Br_S, checkForTermination);

            gen.MarkLabel(doWork);

            gen.Emit(OpCodes.Ldloc, result);
            gen.Emit(OpCodes.Ldloc, accumulator);

            gen.Emit(OpCodes.Ldloc, tmpArray);
            gen.Emit(OpCodes.Ldloc, accumulator);
            gen.Emit(OpCodes.Callvirt, JArrayItemInt32.GetGetMethod()); // JToken <noname> = jarray[accumulator];

            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Call, typeof(EntityFromTokenFactory<>).MakeGenericType(prop.PropertyType.GetElementType()).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));
                // var <noname-entity> = EntityFromTokenFactory<Type>.Create(<noname>, session, true);
            gen.Emit(OpCodes.Stelem_Ref);
            
            // increment
            gen.Emit(OpCodes.Ldloc, accumulator);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Add);
            gen.Emit(OpCodes.Stloc, accumulator);

            gen.MarkLabel(checkForTermination);
            gen.Emit(OpCodes.Ldloc, accumulator);
            gen.Emit(OpCodes.Ldloc, count);
            gen.Emit(OpCodes.Clt);
            gen.Emit(OpCodes.Brtrue_S, doWork);

            // assign
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, result);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());

            gen.MarkLabel(notSetting);
            gen.Emit(OpCodes.Nop);
        }

        private static void GenerateILForStringProperty(ILGenerator gen, PropertyInfo prop, JsonPropertyAttribute attr)
        {
            string token = attr.PropertyName;
            LocalBuilder local = gen.DeclareLocal(typeof(JToken));
            LocalBuilder result = gen.DeclareLocal(typeof(string));
            Label notSetting = gen.DefineLabel();

            if (token.IndexOf('/') == -1)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, JTokenItem.GetGetMethod());
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, token);
                gen.Emit(OpCodes.Callvirt, WalkToValueCallbackInvoke);
            }

            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, notSetting);

            Label tryCatchICEStart = gen.BeginExceptionBlock(); // try {
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Call, CastJTokenToString);
            gen.Emit(OpCodes.Stloc, result);
            gen.Emit(OpCodes.Leave_S, tryCatchICEStart); // result = (string)local;

            gen.BeginCatchBlock(typeof(ArgumentException)); // } catch (ArgumentException) {
            gen.Emit(OpCodes.Pop); // disregard exception on stack

            Label innerTryStart = gen.BeginExceptionBlock(); // try {
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Callvirt, ObjectToString);
            gen.Emit(OpCodes.Stloc, result); // result = local.ToString();
            gen.Emit(OpCodes.Leave_S, innerTryStart);

            gen.BeginCatchBlock(typeof(object)); // } catch {
            gen.Emit(OpCodes.Pop); // disregard exception on stack
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Stloc, result); // result = null;
            gen.Emit(OpCodes.Leave_S, innerTryStart); // }
            gen.EndExceptionBlock();

            gen.Emit(OpCodes.Leave_S, tryCatchICEStart); // }
            gen.EndExceptionBlock();

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc, result);
            gen.Emit(OpCodes.Callvirt, prop.GetSetMethod());

            gen.MarkLabel(notSetting);
            gen.Emit(OpCodes.Nop);
        }
        #endregion

        #region reference methods
        //private static void ReferenceGenerateStringProperty(JToken source, OpenGraphEntity entity, WalkToValueCallback walker, OpenGraphSession session)
        //{
        //    JToken src = Get(source);
        //    if (src != null)
        //    {
        //        string result = (string)src;
        //        entity.ID = result;
        //    }
        //}

        //private static void ReferenceGenerateOpenGraphTypeProperty<TEntityType>(JToken source, OpenGraphEntity entity, WalkToValueCallback walker, OpenGraphSession session)
        //{
        //    JToken src = Get(source);
        //    OpenGraphEntity result = EntityFromTokenFactory<TEntityType>.Create(src, session, true);
        //    entity.Education = result;
        //}

        //private static void ReferenceGenerateOGATProp(JToken source, OpenGraphEntity entity, WalkToValueCallback walker, OpenGraphSession session)
        //{
        //    JToken src = Get(source); // if/else branch
        //    if (src != null)
        //    {
        //        JArray array = (JArray)src;
        //        int count = array.Count;
        //        Education[] result = new Education[count];
        //        for (int i = 0; i < count; i++)
        //        {
        //            JToken src = array[i];
        //            result[i] = EntityFromTokenFactory<Education>.Create(src, session, true);
        //        }
        //        entity.Education = result;
        //    }
        //}

        //private static void ReferenceGenerateNDTProp(JToken source, OpenGraphEntity entity, WalkToValueCallback walker, OpenGraphSession session)
        //{
        //    JToken src = Get(source); // if/else branch at start
        //    DateTime val = DateTime.MinValue;
        //    if (src != null)
        //    {
        //        string tmp = (string)src;
        //        if (tmp != null)
        //        {
        //            if (DateTime.TryParse(tmp, out val))
        //            {
        //                entity.LastUpdated = val;
        //            notSettingProperty:
        //                ;
        //            }
        //        }
        //    }
        //}


        //private static void ReferenceGenerateConnectionProperty(JToken source, OpenGraphEntity entityToPop, WalkToValueCallback walker, OpenGraphSession session)
        //{
            //entityToPop.Friends = new Connection<Friend>(entityToPop.ID, "friends", source["friends"], session);
        //}
        #endregion
    }
#pragma warning restore 1591
}
