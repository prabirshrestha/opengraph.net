using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Facebook.Graph.Util
{
    internal delegate OpenGraphException CreateOpenGraphExceptionCallback(string exceptionMessage);

    internal static class ExceptionParser
    {
        private static Dictionary<string, Type> typeMap;
        private static Dictionary<Type, CreateOpenGraphExceptionCallback> callbacks;
        private static Dictionary<string, string> AuthErrorResponses = new Dictionary<string, string>
        {
            { "", "An unknown authorization error occurred." },
            { "user_denied", "The user denied authorization for one or more extended permissions." }
        };

        private static void Initialize()
        {
            typeMap = new Dictionary<string, Type>();
            callbacks = new Dictionary<Type, CreateOpenGraphExceptionCallback>();

            Type baseExcType = typeof(OpenGraphException);;

#if !SILVERLIGHT
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (baseExcType.IsAssignableFrom(type))
                    {
                        typeMap.Add(type.Name, type);
                    }
                }
            }
#else
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (baseExcType.IsAssignableFrom(type))
                    typeMap.Add(type.Name, type);
            }
#endif
        }

        public static OpenGraphException Parse(JToken error)
        {
            if (typeMap == null)
            {
                Initialize();
            }

            string type = (string)error["type"];
            string message = (string)error["message"];

            Type excType;
            if (!typeMap.TryGetValue(type, out excType))
            {
                excType = typeof(OpenGraphException);
            }

            CreateOpenGraphExceptionCallback callback;
            if (!callbacks.TryGetValue(excType, out callback))
            {
                callback = CreateDynamicMethod(excType);
                callbacks.Add(excType, callback);
            }

            return callback(message);
        }

        private static CreateOpenGraphExceptionCallback CreateDynamicMethod(Type excType)
        {
            DynamicMethod method = new DynamicMethod("ExceptionParser$CreateException$" + Regex.Replace(excType.FullName, "\\W+", "$"), excType, new Type[] { typeof(string) });
            ILGenerator gen = method.GetILGenerator();

            ConstructorInfo constr = excType.GetConstructor(new Type[] { typeof(string) });
            if (constr == null)
                throw new InvalidCastException("Could not create a dynamic method for type " + excType.FullName + "; a public constructor accepting a single String parameter was not found.");

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Newobj, constr);
            gen.Emit(OpCodes.Ret);

            CreateOpenGraphExceptionCallback result = method.CreateDelegate(typeof(CreateOpenGraphExceptionCallback)) as CreateOpenGraphExceptionCallback;
            return result;
        }

        public static OpenGraphException CreateForAuthErrorReason(string errorReason)
        {
            string message;
            if (!AuthErrorResponses.TryGetValue(errorReason, out message))
            {
                message = AuthErrorResponses[""];
            }

            return new OAuthException(message);
        }
    }
}
