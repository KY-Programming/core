using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KY.Core
{
    public class PropertyChangedForwarder
    {
        private readonly IRaisePropertyChanged raiser;
        private readonly INotifyPropertyChanged notifier;
        private readonly Dictionary<string, List<Action>> listeningProperties;

        public PropertyChangedForwarder(IRaisePropertyChanged raiser, INotifyPropertyChanged notifier)
        {
            if (raiser == null)
                throw new ArgumentNullException(nameof(raiser));
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            this.raiser = raiser;
            this.notifier = notifier;
            this.listeningProperties = new Dictionary<string, List<Action>>();
        }

        public void ForwardPropertyChanged(string notifierProperty, string raiserProperty)
        {
            this.SubscribeNotifierPropertyChangedIfNotAlreadySubscribed();
            this.AddEmptyListeningPropertiesListForNotifier(notifierProperty);
            this.listeningProperties[notifierProperty].Add(() => this.raiser.RaisePropertyChanged(raiserProperty));
        }

        private void AddEmptyListeningPropertiesListForNotifier(string notifierPropertyName)
        {
            if (!this.listeningProperties.ContainsKey(notifierPropertyName))
            {
                this.listeningProperties.Add(notifierPropertyName, new List<Action>());
            }
        }

        private void SubscribeNotifierPropertyChangedIfNotAlreadySubscribed()
        {
            if (this.listeningProperties.Count == 0)
            {
                this.notifier.PropertyChanged += this.OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!this.listeningProperties.ContainsKey(args.PropertyName))
                return;

            this.listeningProperties[args.PropertyName].ForEach(action => action());
        }
    }
}