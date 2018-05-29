using System;

namespace NuGetReporter.Cmdlet
{
    public sealed class Package
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string NewestVersion { get; set; }
    }
}