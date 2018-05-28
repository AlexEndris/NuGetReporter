using System.Linq;
using NuGet.Configuration;
using NuGetReporter.Cmdlet;
using NuGetReporter.Cmdlet.Config;
using NuGetReporter.Cmdlet.Packaging;

namespace NuGetReporter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            NewMethod1();
        }

        private static void NewMethod1()
        {
            var settings = Settings.LoadDefaultSettings(@"<RootDir>");

            var sourceProvider = new PackageSourceProvider(settings);
            var sources = sourceProvider.LoadPackageSources();


            var retriever = new PackageInfoRetriever(sources);

            var package = retriever.GetNewest("NuGet.Client", true);
        }

        private static void NewMethod()
        {
            var factory = new ConfigReaderFactory();

            var reader = factory.Create(ConfigType.CsProj);

//            var packages = reader
//                .Read(@"<PackagesConfig>").ToList();
            var packages = reader
                .Read(@"D:\Development\Programming\Projects\NuGetReporter\NuGetReporter.Cmdlet\NuGetReporter.Cmdlet.csproj")
                .ToList();
        }
    }
}