using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KY.Core.Extension;

public static class TaskExtension
{
    /// <summary>
    /// This method does nothing. It only suppresses warning CS4014
    /// </summary>
    /// <example>[CS4014] Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FireAndForget(this Task _) { }
}
