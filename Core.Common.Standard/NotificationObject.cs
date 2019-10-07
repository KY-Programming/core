using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using KY.Core.Properties;

namespace KY.Core
{
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        [NonSerialized] 
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        ///     Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler changedEventHandler = this.propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref this.propertyChanged, comparand + value, comparand);
                } while (changedEventHandler != comparand);
            }
            remove
            {
                PropertyChangedEventHandler changedEventHandler = this.propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref this.propertyChanged, comparand - value, comparand);
                } while (changedEventHandler != comparand);
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property e.g. nameof(this.Content)</param>
        public void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            if (propertyName == string.Empty)
                throw new ArgumentOutOfRangeException(nameof(propertyName), Resources.ErrorCanNotBeEmpty);
            
            this.propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        [Obsolete]
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            this.RaisePropertyChanged(propertyExpression.ExtractPropertyName());
        }
    }
}