namespace KY.Core.Sync
{
    public interface ISyncable : IMergeable
    {
        #region Properties
        int Id { get; set;  }
        #endregion
    }
}