using System.Collections.Generic;

namespace KY.Core
{
    public static class QueueExtension
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> list)
        {
            list.ForEach(queue.Enqueue);
        }

        public static List<T> DequeueAll<T>(this Queue<T> queue)
        {
            List<T> list = new List<T>();
            while (queue.Count > 0)
            {
                list.Add(queue.Dequeue());
            }
            return list;
        }

        public static T DequeueOrDefault<T>(this Queue<T> queue)
        {
            return queue.Count == 0 ? default(T) : queue.Dequeue();
        }
    }
}
