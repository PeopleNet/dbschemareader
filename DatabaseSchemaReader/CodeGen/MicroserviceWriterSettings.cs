using System.IO;
using System.Collections.Generic;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.CodeGen
{
    public class MicroserviceWriterSettings
    {
        public MicroserviceWriterSettings(
            DatabaseTable table,
            DatabaseSchema schema,
            DirectoryInfo outputDirectory,
            bool initializeGitRepository,
            string repositoryNameTemplate,
            string projectNameTemplate,
            bool createIntegrationTests,
            bool createUnitTests,
            string namespaceForCode,
            string[] logicalDeleteColumns)
        {
            Table = table;
            Schema = schema;
            OutputDirectory = outputDirectory;
            InitializeGitRepository = initializeGitRepository;
            RepositoryNameTemplate = repositoryNameTemplate;
            ProjectNameTemplate = projectNameTemplate;
            CreateIntegrationTests = createIntegrationTests;
            CreateUnitTests = createUnitTests;
            NamespaceForCode = namespaceForCode;
            LogicalDeleteColumns = logicalDeleteColumns;
        }

        public DatabaseTable Table { get; }

        public DatabaseSchema Schema { get; }

        public DirectoryInfo OutputDirectory { get; }

        public bool InitializeGitRepository { get; }

        public string RepositoryNameTemplate { get; }

        public string ProjectNameTemplate { get; }

        public bool CreateIntegrationTests { get; }

        public bool CreateUnitTests { get; }

        public string NamespaceForCode { get; }

        public IEnumerable<string> LogicalDeleteColumns { get; }
    }
}