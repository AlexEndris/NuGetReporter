﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGetReporter.Cmdlet.Config
{
    internal sealed class PackageConfigReader : IConfigReader
    {
        public IEnumerable<Package> Read(string fileName)
        {
            string packageConfig = File.ReadAllText(fileName);

            var xml = XElement.Parse(packageConfig);

            var packageNodes = xml.Elements("package");

            return packageNodes.Select(Deserialize);
        }

        private static Package Deserialize(XElement packageNode)
        {
            string id = packageNode.Attribute("id")?.Value;
            string version = packageNode.Attribute("version")?.Value;

            if (id == null
                || version == null)
                return null;
            
            return new Package
            {
                Id = id,
                Version =version
            };
        }
    }
}