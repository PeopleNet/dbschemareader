using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseSchemaReader.CodeGen
{
    using System.Diagnostics;

    using DatabaseSchemaReader.DataSchema;
    using System.IO;

    using McMaster.Extensions.CommandLineUtils;

    public class DotnetCoreProjectWriter
    {
        public DatabaseSchema Schema { get; }

        public DirectoryInfo OutputDirectory { get; }

        public bool InitializeGitRepository { get; }

        public string RepositoryNameTemplate { get; }

        public string ProjectNameTemplate { get; }

        public bool CreateIntegrationTests { get; }

        public bool CreateUnitTests { get; }

        public DotnetCoreProjectWriter(
            DatabaseSchema schema,
            DirectoryInfo outputDirectory,
            bool initializeGitRepository,
            string repositoryNameTemplate,
            string projectNameTemplate,
            bool createIntegrationTests,
            bool createUnitTests)
        {
            Schema = schema;
            OutputDirectory = outputDirectory;
            InitializeGitRepository = initializeGitRepository;
            RepositoryNameTemplate = repositoryNameTemplate;
            ProjectNameTemplate = projectNameTemplate;
            CreateIntegrationTests = createIntegrationTests;
            CreateUnitTests = createUnitTests;
        }

        public void Execute()
        {
            var pathToDotNetExe = DotNetExe.FullPathOrDefault();
            Process.Start(pathToDotNetExe, "--help");
            
            foreach (var t in Schema.Tables)
            {
                var repositoryName = RepositoryNameTemplate.Replace("<tablename>", t.NetName.ToLower());
                var repositoryPath = Path.Combine(OutputDirectory.FullName, repositoryName);
                var projectName = ProjectNameTemplate.Replace("<tablename>", t.NetName);
                Process.Start(pathToDotNetExe, $"new sln -n \"{projectName}\" -o \"{repositoryPath}\"");
            }
        }
    }
}
