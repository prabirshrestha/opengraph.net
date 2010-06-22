using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using Facebook.OpenGraph.Metadata;

namespace Facebook.Graph.Util
{
    internal static class SubscriberParser
    {
        internal static string ParseNewExpressionIntoResult(NewExpression expression, Type parameterType, out string entityType)
        {
            StringBuilder sb = new StringBuilder();

            var gtna = (parameterType.GetCustomAttributes(typeof(GraphTypeNameAttribute), true) as GraphTypeNameAttribute[]).FirstOrDefault();
            if (gtna != null)
            {
                entityType = gtna.Name;
            }
            else
            {
                entityType = null;
            }

            if (expression.Arguments.Count == 0)
                throw new ArgumentException("Expression contained no output data.", "expression");

            foreach (var arg in expression.Arguments)
            {
                if (arg.NodeType != ExpressionType.MemberAccess)
                    throw new ArgumentException("Could not parse expression node '" + arg.ToString() + "'; only immediate properties of the entity can be retrieved.", "expression");

                MemberExpression memExpr = arg as MemberExpression;
                PropertyInfo interestingProp = memExpr.Member as PropertyInfo;

                if (interestingProp == null)
                    throw new ArgumentException("Could not access expression node '" + memExpr.Member.Name + "' because it was not a property.");

                var propAttr = (interestingProp.GetCustomAttributes(typeof(JsonPropertyAttribute), true) as JsonPropertyAttribute[]).FirstOrDefault();
                if (propAttr == null)
                    continue;

                sb.AppendProperty(propAttr.PropertyName);
            }

            return sb.ToString();
        }

        internal static string ParseEntityExpressionIntoResult(ParameterExpression expression, out string entityType)
        {
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo property in expression.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                JsonPropertyAttribute attr = (property.GetCustomAttributes(typeof(JsonPropertyAttribute), true) as JsonPropertyAttribute[]).FirstOrDefault();
                if (attr != null)
                {
                    sb.AppendProperty(attr.PropertyName);
                }
            }

            var gtna = (expression.Type.GetCustomAttributes(typeof(GraphTypeNameAttribute), true) as GraphTypeNameAttribute[]).FirstOrDefault();
            if (gtna != null)
            {
                entityType = gtna.Name;
            }
            else
            {
                entityType = null;
            }

            return sb.ToString();
        }

        private static void AppendProperty(this StringBuilder sb, string addition)
        {
            if (sb.Length > 0)
                sb.AppendFormat(",{0}", addition);
            else
                sb.Append(addition);
        }
    }
}
