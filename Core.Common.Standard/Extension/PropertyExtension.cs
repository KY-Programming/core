using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace KY.Core
{
    public static class PropertyExtension
    {
        public static string ExtractPropertyName<T>(this Expression<T> propertyExpression)
        {
            if (propertyExpression == null)
                return null;

            return ExtractMemberExpression(propertyExpression).Member.Name;
        }

        private static MemberExpression ExtractMemberExpression<T>(Expression<T> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = ExtractMemberExpression(propertyExpression.Body);
            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            return memberExpression;
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
                return ExtractMemberExpression(unaryExpression.Operand);

            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
                return ExtractMemberExpression(binaryExpression.Left);

            throw new InvalidOperationException(/*Properties.Resources.ExceptionTextUnknownExpressionType*/);
        }

        public static List<MemberExpression> CollectMemberExpressions<T>(this Expression<T> expression)
        {
            return CollectMemberExpressions(expression.Body);
        }

        public static List<MemberExpression> CollectMemberExpressions(this Expression expression)
        {
            var expressions = new List<MemberExpression>();

            if (expression is MemberExpression)
                expressions.Add((MemberExpression)expression);
            if (expression is UnaryExpression)
                expressions.AddRange(CollectMemberExpressions(((UnaryExpression)expression).Operand));
            if (expression is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression)expression;
                expressions.AddRange(CollectMemberExpressions(binaryExpression.Left));
                expressions.AddRange(CollectMemberExpressions(binaryExpression.Right));
            }

            return expressions;
        }

        public static string ExtractPropertyName(this MemberExpression expression)
        {
            return expression.Member.Name;
        }

        public static TReturn ExtractValue<TReturn, TExpression>(this Expression<Func<TExpression>> expression, int exitLevel = 0)
        {
            return ExtractValue<TReturn>((MemberExpression)expression.Body, exitLevel);
        }

        public static T ExtractValue<T>(this MemberExpression expression, int exitLevel = 0, int currentLevel = 0)
        {
            string field = ExtractPropertyName(expression);
            object source;
            if (expression.Expression is ConstantExpression)
                source = ((ConstantExpression)expression.Expression).Value;
            else if (expression.Expression is MemberExpression)
                source = ExtractValue<object>((MemberExpression)expression.Expression, exitLevel, currentLevel + 1);
            else
                return default(T);

            if (currentLevel < exitLevel)
                return (T)source;

            PropertyInfo propertyInfo = source.GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null)
                return (T)propertyInfo.GetValue(source, null);
            FieldInfo fieldInfo = source.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
                return (T)fieldInfo.GetValue(source);
            return default(T);
        }
    }
}