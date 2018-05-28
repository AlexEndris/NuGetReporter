using System.Linq;
using NuGet.Configuration;
using NuGetReporter.Cmdlet;
using NuGetReporter.Cmdlet.Config;
using NuGetReporter.Cmdlet.Packaging;

namespace NuGetReporter.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json", "nuget.org");

            var retriever = new PackageInfoRetriever(new[] {packageSource});

            var package = retriever.GetNewest("NuGet.Client", true);
        }

        private static void NewMethod()
        {
            var factory = new ConfigReaderFactory();

            var reader = factory.Create(ConfigType.CsProj);

//            var packages = reader
//                .Read(@"D:\Development\Programming\Git Repos\leaguestats\LeagueStats.Core\packages.config").ToList();
            var packages = reader
                .Read(@"D:\Development\Programming\Projects\NuGetReporter\NuGetReporter.Cmdlet\NuGetReporter.Cmdlet.csproj")
                .ToList();
        }
    }
}