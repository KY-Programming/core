using System;
using System.ServiceModel;
using System.Threading;

namespace KY.Core
{
    public abstract class ChannelFacade<TChannel>
    {
        private bool wasOnline;
        private TChannel _NOTUSE_Channel;
        private ICommunicationObject _NOTUSE_CommunicationObject;
        private readonly object connectionMutex = new object();

        protected TChannel Channel
        {
            get { return this._NOTUSE_Channel; }
            private set
            {
                this._NOTUSE_Channel = value;
                this._NOTUSE_CommunicationObject = (ICommunicationObject)value;
            }
        }

        protected ICommunicationObject CommunicationObject
        {
            get { return this._NOTUSE_CommunicationObject; }
        }

        public bool IsOnline
        {
            get
            {
                lock (this.connectionMutex)
                {
                    return !Equals(this.Channel, default(TChannel)) &&
                           (this.CommunicationObject.State == CommunicationState.Opened || this.CommunicationObject.State == CommunicationState.Created);
                }
            }
        }

        public event EventHandler GoesOnline;
        public event EventHandler GoesOffline;

        [Obsolete]
        protected virtual void OnConnecting()
        { }

        [Obsolete]
        protected virtual void OnGoesOnline()
        { }

        [Obsolete]
        protected virtual void OnGoesOffline()
        { }

        public void Open()
        {
            this.OnConnecting();
            lock (this.connectionMutex)
            {
                this.Channel = this.CreateChannel();
            }

            if(Equals(this.Channel, default(TChannel)))
                return;

            this.CommunicationObject.Opened += this.OnChannelOpened;
            this.CommunicationObject.Faulted += this.OnChannelFaulted;
            this.CommunicationObject.Closed += this.OnChannelClosed;
            this.OnChannelOpened(null, null);
        }

        protected abstract TChannel CreateChannel();

        public void Close()
        {
            lock (this.connectionMutex)
            {
                this.CloseChannel();
            }
            this.OnGoesOffline();
        }

        private void OnChannelOpened(object sender, EventArgs e)
        {
            this.wasOnline = true;
            this.OnGoesOnline();
            if (this.GoesOnline != null)
            {
                this.GoesOnline(this, EventArgs.Empty);
            }
        }

        private void OnChannelFaulted(object sender, EventArgs eventArgs)
        {
            this.CloseChannel();
        }

        private void OnChannelClosed(object sender, EventArgs eventArgs)
        {
            this.CleanupChannel();
        }

        protected void CloseChannel()
        {
            if (Equals(this.Channel, default(TChannel)))
                return;

            this.CommunicationObject.CloseSafe();
            this.CleanupChannel();
        }

        private void CleanupChannel()
        {
            if (Equals(this.Channel, default(TChannel)))
                return;

            this.CommunicationObject.Faulted -= this.OnChannelFaulted;
            this.CommunicationObject.Closed -= this.OnChannelClosed;
            this.Channel = default(TChannel);

            if (this.wasOnline && this.GoesOffline != null)
            {
                this.GoesOffline(this, EventArgs.Empty);
            }
            this.wasOnline = false;
        }

        private bool PreExecuteChecks()
        {
            //Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA, "Operation should not running on UiThread");

            lock (this)
            {
                if (!this.IsOnline)
                {
                    this.Open();
                    return this.IsOnline;
                }
                return true;
            }
        }

        protected bool ExecuteSafe(Action action, int tries = 1)
        {
            if(tries == 0)
                throw new ArgumentOutOfRangeException("tries");

            while (tries != 0)
            {
                try
                {
                    if (this.PreExecuteChecks())
                    {
                        action();
                        return true;
                    }
                    return false;
                }
                catch /*(CommunicationException)*/ (Exception exception)
                {
                    this.CloseChannel();
                    //TODO: Exception an die UI weitergeben
                    Logger.Error(exception);
                }
                if (tries > 0)
                {
                    tries--;
                }
                if (tries > 0)
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }

        protected IPromise<T> ExecuteSafe<T>(Func<T> action, int tries = 1)
        {
            T result = default(T);
            bool success = this.ExecuteSafe(new Action(() => result = action()), tries);
            if (success)
                return Deferred.Resolved(result);
            return Deferred.Rejected<T>();
        }

        public IPromise ExecuteSafe(Action<TChannel> action, int tries = 1)
        {
            bool success = this.ExecuteSafe(() => action(this.Channel), tries);
            if (success)
                return Deferred.Resolved();
            return Deferred.Rejected();
        }

        public IPromise<T> ExecuteSafe<T>(Func<TChannel, T> action, int tries = 1)
        {
            return this.ExecuteSafe(() => action(this.Channel), tries);
        }
    }
}