using DatabaseSchemaReader.DataSchema;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseSchemaReader.CodeGen
{
    public class DotnetCoreIntegrationTestProjectWriter : DotnetCoreWriter
    {
        public DotnetCoreIntegrationTestProjectWriter(DotnetCoreWriterSettings settings) : base(settings)
        {
        }

        public override Task Execute()
        {
            return StartDotNetProcess($"new xunit -n {ProjectName}.IntegrationTests -o {Path.Combine(RepositoryPath, ProjectName + ".IntegrationTests")}");
        }
    }
}
