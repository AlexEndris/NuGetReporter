using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using NuGet.Configuration;

namespace NuGetReporter.Cmdlet.PowerShell
{
	[Cmdlet("List", "Updates")]
	public class ListUpdatesCommand : System.Management.Automation.Cmdlet
	{
		[Parameter]
		public string RootFolder { get; set; } = Environment.CurrentDirectory;

		[Parameter]
		[Alias("o")]
		public string Output { get; set; } = $"{Path.Combine(Environment.CurrentDirectory, "nugetreport.json")}";
		
		protected override void ProcessRecord()
		{
			var sources = GetPackageSources(RootFolder);
			
			
			
			base.ProcessRecord();
		}

		private static IEnumerable<PackageSource> GetPackageSources(string directory)
		{
			var settings = Settings.LoadDefaultSettings(directory);

			var sourceProvider = new PackageSourceProvider(settings);
			return sourceProvider.LoadPackageSources();
		}

		private static IEnumerable<Package> GetPackages(string directory)
		{
			Directory.EnumerateFiles(directory, "*.config", SearchOption.AllDirectories)
				.Union(Directory.EnumerateFiles(directory, "*.csproj", SearchOption.AllDirectories));

			return null;
		}
	}
}