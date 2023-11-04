using System.Collections.Generic;

namespace KY.Core;

public interface IAssemblyCache
{
    Dictionary<string, string> Global { get; set; }
    Dictionary<string, string> Local { get; set; }
}
