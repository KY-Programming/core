using System.Collections.Generic;

namespace KY.Core;

public interface IAssemblyCache
{
    void Add(string name, string location);
    string Resolve(string name);
}
