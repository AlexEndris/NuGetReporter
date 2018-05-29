using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGetReporter.Cmdlet.Config
{
	public class ConfigTypeDeterminer
	{
		public ConfigType Determine(string fileName)
		{
			string packageConfig = File.ReadAllText(fileName);

			var xml = XElement.Parse(packageConfig);

			if (xml.DescendantsAndSelf(XmlConstants.Package).Any())
				return ConfigType.PackageConfig;

			if (xml.DescendantsAndSelf(XmlConstants.PackageReference).Any())
				return ConfigType.CsProj;
			
			throw new NotSupportedException("Couldn't determine what file type is being used.");
		}
	}
}