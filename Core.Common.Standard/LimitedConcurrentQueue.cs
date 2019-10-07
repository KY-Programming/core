using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KY.Core
{
    public class LimitedConcurrentQueue<T> : IProducerConsumerCollection<T>
    {
        private readonly int maxItems;
        private readonly ConcurrentQueue<T> queue;

        public bool IsEmpty
        {
            get { return this.queue.IsEmpty; }
        }

        public int Count
        {
            get { return this.queue.Count; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)this.queue).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)this.queue).IsSynchronized; }
        }

        public LimitedConcurrentQueue(int maxItems)
        {
            this.maxItems = maxItems;
            this.queue = new ConcurrentQueue<T>();
        }

        public LimitedConcurrentQueue(int maxItems, IEnumerable<T> collection)
        {
            this.maxItems = maxItems;
            this.queue = new ConcurrentQueue<T>(collection);
            this.DequeueToMaxItems();
        }

        private void DequeueToMaxItems()
        {
            while (this.Count > this.maxItems)
            {
                T dummy;
                this.TryDequeue(out dummy);
            }
        }

        public void Enqueue(T item)
        {
            this.queue.Enqueue(item);
            this.DequeueToMaxItems();
        }

        public void Enqueue(IEnumerable<T> items)
        {
            items.ForEach(this.Enqueue);
        }

        public bool TryDequeue(out T result)
        {
            return this.queue.TryDequeue(out result);
        }

        public IEnumerable<T> DequeueAll()
        {
            var list = new List<T>();
            while (!this.IsEmpty)
            {
                T item;
                if (this.TryDequeue(out item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public bool TryPeek(out T result)
        {
            return this.queue.TryPeek(out result);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)this.queue).CopyTo(array, index);
        }

        public void CopyTo(T[] array, int index)
        {
            this.queue.CopyTo(array, index);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            return ((IProducerConsumerCollection<T>)this.queue).TryAdd(item);
        }

        public bool TryTake(out T item)
        {
            return ((IProducerConsumerCollection<T>)this.queue).TryTake(out item);
        }

        public T[] ToArray()
        {
            return this.queue.ToArray();
        }
    }
}