using System.Collections;
using System.Collections.Specialized;

namespace KY.Core.Sync
{
    public interface IRemoteSyncCallback
    {
        void CollectionChanged(object id, NotifyCollectionChangedAction action, int newStartingIndex, IList newItems, int oldStartingIndex, IList oldItems);
        void PropertyChanged(object id, string property, ISyncable changedItem);
    }
}
