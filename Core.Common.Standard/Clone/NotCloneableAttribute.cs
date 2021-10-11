using System;

// ReSharper disable once CheckNamespace : Lower namespace for easier usage
namespace KY.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotCloneableAttribute : Attribute
    {
    }
}
