using System.Reflection;

namespace KY.Core.Clone
{
    internal class DelayedSetter
    {
        public MethodInfo Setter { get; }
        public object Source { get; }
        public object Target { get; }

        public DelayedSetter(MethodInfo setter, object source, object target)
        {
            this.Setter = setter;
            this.Source = source;
            this.Target = target;
        }
    }
}