using System;
using NuGetReporter.Cmdlet.Config;

namespace NuGetReporter.Cmdlet
{
    public sealed class ConfigReaderFactory
    {
        public IConfigReader Create(ConfigType type)
        {
            switch (type)
            {
                case ConfigType.PackageConfig:
                    return new PackageConfigReader();
                case ConfigType.CsProj:
                    return new CsProjReader();
                case ConfigType.ProjectJson:
                    throw new NotImplementedException("Project.json is pretty obsolete now. Not implemented yet.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}