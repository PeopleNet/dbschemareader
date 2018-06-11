using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseSchemaReader.CodeGen
{
    using System.IO;

    using DatabaseSchemaReader.DataSchema;

    public class DotnetCoreWriterSettings
    {
        public DatabaseTable Table { get; }
        public DirectoryInfo OutputDirectory { get; }
        public string RepositoryNameTemplate { get; }
        public string ProjectNameTemplate { get; }

        public DotnetCoreWriterSettings(
            DatabaseTable table,
            DirectoryInfo outputDirectory,
            string repositoryNameTemplate,
            string projectNameTemplate)
        {
            Table = table;
            OutputDirectory = outputDirectory;
            RepositoryNameTemplate = repositoryNameTemplate;
            ProjectNameTemplate = projectNameTemplate;
        }
    }
}
