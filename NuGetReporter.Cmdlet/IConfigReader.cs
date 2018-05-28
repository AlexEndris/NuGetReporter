using System.Collections.Generic;

namespace NuGetReporter.Cmdlet
{
    public interface IConfigReader
    {
        IEnumerable<Package> Read(string fileName);
    }
}