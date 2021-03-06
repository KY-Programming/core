﻿using System;

namespace KY.Core
{
    internal class AssemblyInfo
    {
        public string Name { get; }
        public Version Version { get; }
        public string Path { get; }
        public bool IsExecutable { get; }
        public bool IsResource { get; }

        public AssemblyInfo(string name, Version version = null, string path = null)
        {
            this.Name = name;
            this.Version = version;
            this.Path = path ?? name;
            this.IsExecutable = this.Name.EndsWith(".exe") || this.Path.EndsWith(".exe");
            this.IsResource = this.Name.EndsWith(".resources") || this.Path.EndsWith(".resources");
        }

        public override string ToString()
        {
            return $"{this.Name}{(this.Version != null ? $"-{this.Version}" : "")}";
        }
    }
}