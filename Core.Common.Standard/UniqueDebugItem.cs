using System;

namespace KY.Core
{
    [Serializable]
    public class UniqueDebugItem
    {
#if DEBUG
        [NonSerialized]
        private Guid _NOTUSE_InternalUniqueId;

        public Guid InternalUniqueId
        {
            get
            {
                if (this._NOTUSE_InternalUniqueId == Guid.Empty)
                    this._NOTUSE_InternalUniqueId = Guid.NewGuid();
                return this._NOTUSE_InternalUniqueId;
            }
        }
#endif
    }
}