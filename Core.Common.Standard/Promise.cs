using System;
using System.Threading;

namespace KY.Core
{
    public interface IPromise
    {
        IPromise Done(Action action);
        IPromise Fail(Action action);
        IPromise Always(Action action);
        void Wait();
    }

    public interface IPromise<out T>
    {
        IPromise<T> Done(Action<T> action);
        IPromise<T> Fail(Action action);
        IPromise<T> Always(Action action);
        T Wait();
    }

    public class Deferred : IPromise
    {
        private readonly ManualResetEvent waitEvent;

        private Action successAction;
        private Action failAction;
        private Action alwaysAction;

        private bool isRejected;
        private bool isResolved;

        private Deferred()
        {
            this.waitEvent = new ManualResetEvent(false);
        }

        public IPromise Resolve()
        {
            if (this.isResolved)
                throw new InvalidOperationException("Deferred already resolved");
            if (this.isRejected)
                throw new InvalidOperationException("Deferred already rejected");

            this.isResolved = true;
            this.successAction?.Invoke();
            this.alwaysAction?.Invoke();
            this.waitEvent.Set();
            return this.Promise();
        }

        public IPromise Reject()
        {
            if (this.isResolved)
                throw new InvalidOperationException("Deferred already resolved");
            if (this.isRejected)
                throw new InvalidOperationException("Deferred already rejected");

            this.isRejected = true;
            this.failAction?.Invoke();
            this.alwaysAction?.Invoke();
            this.waitEvent.Set();
            return this.Promise();
        }

        public IPromise Promise()
        {
            return this;
        }

        IPromise IPromise.Done(Action action)
        {
            if (this.successAction != null)
                throw new InvalidOperationException("Success action already set");

            this.successAction = action;
            if (this.isResolved)
            {
                this.successAction();
            }
            return this;
        }

        IPromise IPromise.Fail(Action action)
        {
            if (this.failAction != null)
                throw new InvalidOperationException("Fail action already set");

            this.failAction = action;
            if (this.isRejected)
            {
                this.failAction();
            }
            return this;
        }

        IPromise IPromise.Always(Action action)
        {
            if (this.alwaysAction != null)
                throw new InvalidOperationException("Always action already set");

            this.alwaysAction = action;
            if (this.isResolved || this.isRejected)
            {
                this.alwaysAction();
            }
            return this;
        }

        public void Wait()
        {
            this.waitEvent.WaitOne();
        }

        public static Deferred Create()
        {
            return new Deferred();
        }

        public static IPromise Resolved()
        {
            return new Deferred().Resolve();
        }

        public static IPromise Rejected()
        {
            return new Deferred().Reject();
        }

        public static Deferred<T> Create<T>()
        {
            return new Deferred<T>();
        }

        public static IPromise<T> Resolved<T>(T value)
        {
            return new Deferred<T>().Resolve(value);
        }

        public static IPromise<T> Rejected<T>()
        {
            return new Deferred<T>().Reject();
        }
    }

    public class Deferred<T> : IPromise<T>
    {
        private readonly ManualResetEvent waiEvent;

        private Action<T> successAction;
        private Action failAction;
        private Action alwaysAction;

        private bool isRejected;
        private bool isResolved;
        private T resolvedValue;

        internal Deferred()
        {
            this.waiEvent = new ManualResetEvent(false);
        }

        public IPromise<T> Resolve(T value)
        {
            if (this.isResolved)
                throw new InvalidOperationException("Deferred already resolved");
            if (this.isRejected)
                throw new InvalidOperationException("Deferred already rejected");

            this.isResolved = true;
            this.resolvedValue = value;
            this.successAction?.Invoke(value);
            this.alwaysAction?.Invoke();
            this.waiEvent.Set();
            return this.Promise();
        }

        public IPromise<T> Reject()
        {
            if (this.isResolved)
                throw new InvalidOperationException("Deferred already resolved");
            if (this.isRejected)
                throw new InvalidOperationException("Deferred already rejected");

            this.isRejected = true;
            this.failAction?.Invoke();
            this.alwaysAction?.Invoke();
            this.waiEvent.Set();
            return this.Promise();
        }

        public IPromise<T> Promise()
        {
            return this;
        }

        IPromise<T> IPromise<T>.Done(Action<T> action)
        {
            if (this.successAction != null)
                throw new InvalidOperationException("Success action already set");

            this.successAction = action;
            if (this.isResolved)
            {
                this.successAction(this.resolvedValue);
            }
            return this;
        }

        IPromise<T> IPromise<T>.Fail(Action action)
        {
            if (this.failAction != null)
                throw new InvalidOperationException("Fail action already set");

            this.failAction = action;
            if (this.isRejected)
            {
                this.failAction();
            }
            return this;
        }

        IPromise<T> IPromise<T>.Always(Action action)
        {
            if (this.alwaysAction != null)
                throw new InvalidOperationException("Always action already set");

            this.alwaysAction = action;
            if (this.isResolved || this.isRejected)
            {
                this.alwaysAction();
            }
            return this;
        }

        public T Wait()
        {
            this.waiEvent.WaitOne();
            return this.resolvedValue;
        }
    }
}