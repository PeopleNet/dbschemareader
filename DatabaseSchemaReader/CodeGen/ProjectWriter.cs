using System;
using System.Linq;
//Reflection needed for GetTypeInfo()
// ReSharper disable once RedundantUsingDirective
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace DatabaseSchemaReader.CodeGen
{
    class ProjectWriter
    {
        private readonly XNamespace _xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
        private readonly XDocument _document;
        private bool _hasOracle;

        public ProjectWriter(string @namespace, ProjectVersion projectVersion)
        {
            _document = LoadProjectXml();
            //give it a unique guid
            var guid = "{" + Guid.NewGuid().ToString("D") + "}";
            var projectGuid = _document.Descendants(_xmlns + "ProjectGuid").First();
            projectGuid.SetValue(guid);

            var rootNamespace = _document.Descendants(_xmlns + "RootNamespace").First();
            rootNamespace.SetValue(@namespace ?? "Project");

            var assemblyName = _document.Descendants(_xmlns + "AssemblyName").First();
            Upgrade(projectVersion);
        }

        private void Upgrade(ProjectVersion projectVersion)
        {
            if (projectVersion == ProjectVersion.Vs2008) return;
            var projectElement = _document.Root;
            if (projectElement == null) return;
            projectElement.SetAttributeValue("ToolsVersion", projectVersion == ProjectVersion.Vs2015 ? "14.0" : "4.0");
            var target = projectElement.Descendants(_xmlns + "TargetFrameworkVersion").First();
            target.SetValue(projectVersion == ProjectVersion.Vs2015 ? "v4.6.1" : "v4.0");
            var systemCore = _document
                .Descendants(_xmlns + "Reference")
                .FirstOrDefault(r => (string)r.Attribute("Include") == "System.Core");
            if (systemCore != null)
            {
                if (projectVersion == ProjectVersion.Vs2010)
                {
                    systemCore.Descendants(_xmlns + "RequiredTargetFramework").First().Remove();
                }
                if (projectVersion == ProjectVersion.Vs2015)
                {
                    systemCore.Remove();
                }
            }
        }

        private static XDocument LoadProjectXml()
        {
            const string streamPath = "DatabaseSchemaReader.CodeGen.Project.xml";
#if !COREFX
            var executingAssembly = typeof(ProjectWriter).Assembly;
#else
            var executingAssembly = typeof(ProjectWriter).GetTypeInfo().Assembly;
#endif
            var stream = executingAssembly.GetManifestResourceStream(streamPath);
            if (stream == null) return null;
            return XDocument.Load(XmlReader.Create(stream));
        }

    }
}
