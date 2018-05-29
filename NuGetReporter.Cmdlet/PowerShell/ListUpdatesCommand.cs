﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGetReporter.Cmdlet.Config;
using NuGetReporter.Cmdlet.Packaging;

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
			var packages = GetPackages(RootFolder);

			AddNewestVersion(packages);
			
			WriteObject(packages, true);
		}

		private void AddNewestVersion(IDictionary<string, IEnumerable<Package>> packages)
		{
			var sources = GetPackageSources(RootFolder);
			var feeds = GetSourceFeeds(sources);

			var infoRetriever = new PackageInfoRetriever(feeds);

			foreach (var package in packages.SelectMany(x => x.Value))
			{
				// TODO: Use switch
				infoRetriever.GetNewest(package, false);
			}
		}

		private static IEnumerable<PackageSource> GetPackageSources(string directory)
		{
			var settings = Settings.LoadDefaultSettings(directory);

			var sourceProvider = new PackageSourceProvider(settings);
			return sourceProvider.LoadPackageSources();
		}

		private IEnumerable<ListResource> GetSourceFeeds(IEnumerable<PackageSource> packageSources)
		{
			var listResources = new HashSet<string>();

			var sourceFeeds = new List<ListResource>();
			foreach (var source in packageSources)
			{
				var packageSource = Repository.Factory.GetCoreV3(source.Source);
				packageSource.PackageSource.Credentials = source.Credentials;
				ListResource resource = null;
                
				try
				{
					resource = packageSource.GetResource<ListResource>();
				}
				catch (Exception e)
				{
					// TODO: Log? Output?
					Console.WriteLine(e);
				}
				if (resource == null) 
					continue;
                
				if (listResources.Add(resource.Source))
					sourceFeeds.Add(resource);
			}

			return sourceFeeds;
		}

		private static IDictionary<string, IEnumerable<Package>> GetPackages(string directory)
		{
			var files = Directory.EnumerateFiles(directory, "*.config", SearchOption.AllDirectories)
				.Union(Directory.EnumerateFiles(directory, "*.csproj", SearchOption.AllDirectories))
				.ToList();

			var readerFactory = new ConfigReaderFactory();
			var typeDeterminer = new ConfigTypeDeterminer();

			var packages = new Dictionary<string, IEnumerable<Package>>();
			
			foreach (string file in files)
			{
				var type = typeDeterminer.Determine(file);
				var readPackages = readerFactory.Create(type)
											.Read(file);
				
				packages.Add(file, readPackages);
			}
			
			return packages;
		}
	}
}