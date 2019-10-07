using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KY.Core
{
    public static class ParallelAsync
    {
        public static Task<ParallelLoopResult> ForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            return Task<ParallelLoopResult>.Factory.StartNew(() => Parallel.ForEach(source, action));
        }
    }
}