using System;
using System.Collections.Generic;

namespace KY.Core
{
    public interface IRuleConverter<in TSource, out TTarget>
    {
        bool CanConvert(TSource source);
        TTarget Convert(TSource source);
    }

    public class RuleConverter<TSource, TTarget> : IRuleConverter<TSource, TTarget>
    {
        private readonly IDictionary<TSource, TTarget> rules;

        protected IDictionary<TSource, TTarget> Rules
        {
            get { return this.rules; }
        }

        protected RuleConverter()
        {
            this.rules = new Dictionary<TSource, TTarget>();
        }

        public RuleConverter(IDictionary<TSource, TTarget> rules)
        {
            this.rules = rules;
        }

        public bool CanConvert(TSource source)
        {
            return this.Rules.ContainsKey(source);
        }

        public TTarget Convert(TSource source)
        {
            if (this.Rules.ContainsKey(source))
                return this.Rules[source];
            return this.DefaultConversion(source);
        }

        protected virtual TTarget DefaultConversion(TSource source)
        {
            throw new InvalidOperationException("Can not convert value " + source);
        }
    }
}