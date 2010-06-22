using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Facebook.Graph.Util
{
    internal static class GraphExpressionParser
    {
        /// <summary>
        /// Retrieves a field list from an expression tree.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal static string ParseRequestExpression<TSource, TResult>(Expression<Func<TSource, TResult>> selector, out string entityType)
            where TSource : GraphEntity
        {
            if (selector.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Must specify a lambda expression with the entity type being viewed and either a parameter or constructor expression as the right-hand result of the lambda expression.", "fieldSelector");

            string fieldsList = null;
            if (selector.Body.NodeType == ExpressionType.New)
            {
                NewExpression result = (NewExpression)selector.Body;
                fieldsList = SubscriberParser.ParseNewExpressionIntoResult(result, selector.Parameters[0].Type, out entityType);
            }
            else if (selector.Body.NodeType == ExpressionType.Parameter)
            {
                fieldsList = SubscriberParser.ParseEntityExpressionIntoResult((ParameterExpression)selector.Body, out entityType);
            }
            else
            {
                throw new ArgumentException("Must specify a lambda expression with the entity type being viewed and either a parameter or constructor expression as the right-hand result of the lambda expression.", "fieldSelector");
            }
            return fieldsList;
        }
    }
}
