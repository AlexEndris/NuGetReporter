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
        private readonly IEnumerable<PackageSource> _packageSources;

        public PackageInfoRetriever(IEnumerable<PackageSource> packageSources)
        {
            _packageSources = packageSources;
        }
        
        public Package GetNewest(string id, bool allowPrerelease)
        {
            var feeds = GetSourceFeeds();
            var logger = NullLogger.Instance;

            var allPackages = new List<IPackageSearchMetadata>();
            
            foreach (var feed in feeds)
            {
                var packagesAsync = feed.ListAsync(id, allowPrerelease, false, false, logger, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

                var packages = ConvertToEnumerable(packagesAsync)
                    .Where(x => string.Equals(x.Identity.Id, id, StringComparison.InvariantCulture))
                    .ToList();
                
                allPackages.AddRange(packages);
            }

            var newestPackage = allPackages.MaxBy(x => x.Identity.Version);
            
            return new Package
            {
                Id = newestPackage.Identity.Id,
                Version = newestPackage.Identity.Version.ToNormalizedString()
            };
        }

        private IEnumerable<IPackageSearchMetadata> ConvertToEnumerable(IEnumerableAsync<IPackageSearchMetadata> packagesAsync)
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

        private IEnumerable<ListResource> GetSourceFeeds()
        {
            var sources = new HashSet<string>();

            return _packageSources
                .Select(source => Repository.Factory.GetCoreV3(source.Source))
                .Select(packageSource => packageSource.GetResource<ListResource>())
                .Where(resource => resource != null)
                .Where(resource => sources.Add(resource.Source))
                .ToList();
        }
    }
}