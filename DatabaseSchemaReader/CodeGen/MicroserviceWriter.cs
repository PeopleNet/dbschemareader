using DatabaseSchemaReader.DataSchema;
using System.IO;

namespace DatabaseSchemaReader.CodeGen
{
    using System.Threading.Tasks;

    public class MicroserviceWriter : IWriter
    {
        private readonly MicroserviceWriterSettings settings;

        public MicroserviceWriter(MicroserviceWriterSettings settings)
        {
            this.settings = settings;
        }

        public async Task Execute()
        {
            var dotnetCoreWriterSettings = new DotnetCoreWriterSettings(settings.Table, settings.OutputDirectory, settings.RepositoryNameTemplate, settings.ProjectNameTemplate);
            var codeWriterSettings = new CodeWriterSettings(Path.Combine(settings.OutputDirectory, ), settings.LogicalDeleteColumns)
                                         {
                                             CodeTarget = CodeTarget.Poco,
                                             Namespace = settings.NamespaceForCode,
                                         };

            var solutionWriter = new DotnetCoreSolutionWriter(dotnetCoreWriterSettings);
            var webApiWriter = new DotnetCoreWebApiProjectWriter(dotnetCoreWriterSettings);
            var unitTestWriter = new DotnetCoreUnitTestProjectWriter(dotnetCoreWriterSettings);
            var integrationTestWriter = new DotnetCoreIntegrationTestProjectWriter(dotnetCoreWriterSettings);
            var codeWriter = new CodeWriter(settings.Schema, codeWriterSettings);

            await unitTestWriter.Execute();
            await solutionWriter.Execute();
            await webApiWriter.Execute();
            await integrationTestWriter.Execute();
            await codeWriter.Execute();
        }
    }
}
