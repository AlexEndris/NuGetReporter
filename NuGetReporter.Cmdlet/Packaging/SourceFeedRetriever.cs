using System;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGetReporter.Cmdlet.Packaging
{
	public class SourceFeedRetriever
	{
		public IEnumerable<ListResource> GetSourceFeeds(IEnumerable<PackageSource> packageSources)
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

	}
}