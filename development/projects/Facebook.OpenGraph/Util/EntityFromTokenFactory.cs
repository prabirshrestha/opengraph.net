using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Reflection;
using Facebook.OpenGraph.Metadata;

namespace Facebook.Graph.Util
{
    /// <summary>
    /// Constructs <see cref="GraphEntity"/> objects from JToken objects for a given session.
    /// </summary>
    /// <remarks>
    /// <para>This class is marked as public as a result of limitations to the dynamic method generation accessibility required in sandboxed dynamic code.  It is not intended to be used by your code.</para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of entity to construct.  This type must derive from <see cref="GraphEntity"/>.</typeparam>
    public static class EntityFromTokenFactory<TEntity>
        where TEntity : GraphEntity
    {
        private delegate TEntity FactoryCallback(JToken source, GraphSession session);
        private static FactoryCallback callback;

        /// <summary>
        /// Constructs a <see cref="GraphEntity"/> object from a JSON source and session.
        /// </summary>
        /// <remarks>
        /// <para>This method is marked as public as a result of limitations to the dynamic method generation accessibility required in sandboxed dynamic code.  It is not intended to be used by your code.</para>
        /// </remarks>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session to bind to the entity.</param>
        /// <param name="isConnection">Whether this is a graph-connected object.</param>
        /// <returns>A <see cref="GraphEntity"/> object.</returns>
        public static TEntity Create(JToken source, GraphSession session, bool isConnection = false)
        {
            if (callback == null)
            {
                Type type = typeof(TEntity);
                DynamicMethod method = new DynamicMethod(
                    "EntityFromTokenFactory$Create$" + Regex.Replace(type.FullName, "\\W+", "$"), 
                    type, 
                    new Type[] { typeof(JToken), typeof(GraphSession) }
                    );
                ILGenerator gen = method.GetILGenerator();

                ConstructorInfo constr = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(JToken), typeof(GraphSession) }, null);
                if (constr == null)
                    throw new InvalidCastException("Could not create a dynamic method for type " + type.FullName + "; a constructor accepting a single JToken parameter was not found.");

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Newobj, constr);
                gen.Emit(OpCodes.Ret);

                FactoryCallback result = method.CreateDelegate(typeof(FactoryCallback)) as FactoryCallback;
                callback = result;
            }

            TEntity entity = callback(source, session);
            entity.IsConnection = isConnection;
            return entity;
        }
    }

    internal static class EntityFromTokenFactory
    {
        private delegate GraphEntity FactoryCallback(JToken source, GraphSession session, bool isConnection = false);
        private static Dictionary<string, FactoryCallback> s_typeMap = Initialize();

        private static Dictionary<string, FactoryCallback> Initialize()
        {
            Dictionary<string, FactoryCallback> result = new Dictionary<string, FactoryCallback>();
            Assembly thisAsm = Assembly.GetExecutingAssembly();
            Type openGraphType = typeof(GraphEntity);
            Type openFactoryType = typeof(EntityFromTokenFactory<>);

            foreach (Type type in thisAsm.GetTypes().Where(t => openGraphType.IsAssignableFrom(t) && !t.Equals(openGraphType)))
            {
                var attr = (type.GetCustomAttributes(typeof(GraphTypeNameAttribute), true) as GraphTypeNameAttribute[]).FirstOrDefault();
                if (attr != null)
                {
                    //result.Add(attr.Name, type);
                    Type closedType = openFactoryType.MakeGenericType(type);
                    MethodInfo method = closedType.GetMethod("Create");
                    var callback = FactoryCallback.CreateDelegate(typeof(FactoryCallback), method) as FactoryCallback;
                    result.Add(attr.Name, callback);
                }
            }

            return result;
        }

        public static GraphEntity Create(string typeName, JToken source, GraphSession session, bool isConnection = false)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            FactoryCallback factory;
            if (s_typeMap.TryGetValue(typeName, out factory))
            {
                GraphEntity entity = factory(source, session, isConnection);
                return entity;
            }

            return null;
        }
    }
}
