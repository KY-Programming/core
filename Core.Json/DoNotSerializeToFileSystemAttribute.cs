using System;

namespace KY.Core;

[AttributeUsage(AttributeTargets.Property)]
public class DoNotSerializeToFileSystemAttribute : Attribute
{ }
