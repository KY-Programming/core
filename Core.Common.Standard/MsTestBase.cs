namespace KY.Core
{
    /// <summary>
    /// Enables Logger output in MsTest classes
    /// </summary>
    public abstract class MsTestBase
    {
        protected MsTestBase()
        {
            Logger.AllTargets.Add(Logger.VisualStudioOutput);
        }
    }
}