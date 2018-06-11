using DatabaseSchemaReader.DataSchema;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseSchemaReader.CodeGen
{
    public class DotnetCoreUnitTestProjectWriter : DotnetCoreWriter
    {
        public DotnetCoreUnitTestProjectWriter(DotnetCoreWriterSettings settings) : base(settings)
        {
        }

        public override Task Execute()
        {
            return StartDotNetProcess($"new xunit -n {ProjectName}.UnitTests -o {Path.Combine(RepositoryPath, ProjectName + ".UnitTests")}");
        }
    }
}
