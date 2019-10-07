using System;
using System.Collections;
using System.Collections.Specialized;

namespace KY.Core.Sync
{
    public interface IRemoteSyncService
    {
        IList Get(object id);
        void Sync(object id, NotifyCollectionChangedAction action, int newStartingIndex, IList newItems, int oldStartingIndex, IList oldItems);
        void Sync(object id, string property, ISyncable changedItem);
        Action<object, NotifyCollectionChangedAction, int, IList, int, IList> CollectionChanged { get; set; }
        Action<object, string, ISyncable> PropertyChanged { get; set; }
    }
}