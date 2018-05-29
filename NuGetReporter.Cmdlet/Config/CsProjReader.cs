using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGetReporter.Cmdlet.Config
{
    internal sealed class CsProjReader : IConfigReader
    {
        public IEnumerable<Package> Read(string fileName)
        {
            string packageConfig = File.ReadAllText(fileName);

            var xml = XElement.Parse(packageConfig);

            var packageNodes = xml.DescendantsAndSelf(XmlConstants.PackageReference);

            return packageNodes.Select(Deserialize).Where(x => x != null);
        }

        private static Package Deserialize(XElement packageNode)
        {
            string id = packageNode.Attribute(XmlConstants.IncludeAttribute)?.Value;
            string version = packageNode.Attribute(XmlConstants.VersionAttributeCsProj)?.Value;

            if (id == null
                || version == null)
                return null;
            
            return new Package
            {
                Id = id,
                Version = version
            };
        }
    }
}