using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace KY.Core
{
    public static class ExpressionExtension
    {
        public static string ExtractPropertyName<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            return ExtractProperty(expression).Name;
        }

        public static PropertyInfo ExtractProperty<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            MemberExpression memberExpression = ExtractMemberExpression(expression);
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("The member access expression does not access a property.", nameof(expression));

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("The referenced property is a static property.", nameof(expression));

            return propertyInfo;
        }

        public static string ExtractFieldName<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            return ExtractField(expression).Name;
        }

        public static FieldInfo ExtractField<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            MemberExpression memberExpression = ExtractMemberExpression(expression);
            var fieldInfo = memberExpression.Member as FieldInfo;
            if (fieldInfo == null)
                throw new ArgumentException("The member access expression does not access a field.", nameof(expression));

            if (fieldInfo.IsStatic)
                throw new ArgumentException("The referenced field is a static field.", nameof(expression));

            return fieldInfo;
        }

        public static MemberInfo ExtractMember<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            if (expression.Body is MethodCallExpression methodCallExpression)
                return methodCallExpression.Method;

            MemberExpression memberExpression = ExtractMemberExpression(expression);
            var memberInfo = memberExpression.Member;
            if (memberInfo == null)
                throw new ArgumentException("The member access expression does not access a member.", nameof(expression));

            if (memberInfo is FieldInfo fieldInfo && fieldInfo.IsStatic
                || memberInfo is PropertyInfo propertyInfo && propertyInfo.GetGetMethod(true).IsStatic
                || memberInfo is MethodInfo methodInfo && methodInfo.IsStatic
                )
                throw new ArgumentException("The referenced member is a static.", nameof(expression));

            return memberInfo;
        }

        public static string ExtractMethodName<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            return ExtractMethod(expression).Name;
        }

        public static MethodInfo ExtractMethod<T>(this Expression<T> expression)
        {
            if (expression == null)
                return null;

            MemberExpression memberExpression = ExtractMemberExpression(expression);
            var methodInfo = memberExpression.Member as MethodInfo;
            if (methodInfo == null)
                throw new ArgumentException("The member access expression does not access a method.", nameof(expression));

            if (methodInfo.IsStatic)
                throw new ArgumentException("The referenced method is a static method.", nameof(expression));

            return methodInfo;
        }

        private static MemberExpression ExtractMemberExpression<T>(Expression<T> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = ExtractMemberExpression(propertyExpression.Body);
            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");

            return memberExpression;
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                    return memberExpression;
                case UnaryExpression unaryExpression:
                    return ExtractMemberExpression(unaryExpression.Operand);
                case BinaryExpression binaryExpression:
                    return ExtractMemberExpression(binaryExpression.Left);
                default:
                    throw new InvalidOperationException( /*Properties.Resources.ExceptionTextUnknownExpressionType*/);
            }
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
