using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MoreLinq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Repositories;

namespace NuGetReporter.Cmdlet.Packaging
{
	public sealed class PackageInfoRetriever
	{
		private readonly IEnumerable<ListResource> _feeds;

		private readonly IDictionary<string, IPackageSearchMetadata> _cache =
			new Dictionary<string, IPackageSearchMetadata>();

		public PackageInfoRetriever(IEnumerable<ListResource> feeds)
		{
			_feeds = feeds;
		}

		public Package GetNewest(Package package, bool allowPrerelease)
		{
			// TODO: Maybe log?
			var logger = NullLogger.Instance;

			if (!_cache.TryGetValue(package.Id, out var newestPackage))
			{
				newestPackage = GetPackageDataFromFeeds(package, allowPrerelease, logger);

				if (newestPackage == null)
					return package;
				
				_cache.Add(package.Id, newestPackage);
			}

			package.NewestVersion = newestPackage.Identity.Version.ToNormalizedString();

			return package;
		}

		private IPackageSearchMetadata GetPackageDataFromFeeds(Package package, bool allowPrerelease, ILogger logger)
		{
			var allPackages = new List<IPackageSearchMetadata>();

			foreach (var feed in _feeds)
			{
				var packagesAsync = feed.ListAsync(package.Id, allowPrerelease, false, false, logger, CancellationToken.None)
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();

				var packages = ConvertToEnumerable(packagesAsync)
					.Where(x => string.Equals(x.Identity.Id, package.Id, StringComparison.InvariantCultureIgnoreCase))
					.ToList();

				allPackages.AddRange(packages);
			}

			return allPackages.Any()
				? allPackages.MaxBy(x => x.Identity.Version)
				: null;
		}

		private static IEnumerable<IPackageSearchMetadata> ConvertToEnumerable(
			IEnumerableAsync<IPackageSearchMetadata> packagesAsync)
		{
			var enumeratorAsync = packagesAsync.GetEnumeratorAsync();

			var packages = new List<IPackageSearchMetadata>();

			if (enumeratorAsync == null)
				return Enumerable.Empty<IPackageSearchMetadata>();

			while (enumeratorAsync.MoveNextAsync().ConfigureAwait(false).GetAwaiter().GetResult())
			{
				packages.Add(enumeratorAsync.Current);
			}

			return packages;
		}
	}
}