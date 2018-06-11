using DatabaseSchemaReader.DataSchema;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseSchemaReader.CodeGen
{
    public class DotnetCoreSolutionWriter : DotnetCoreWriter
    {
        public override async Task Execute() => await StartDotNetProcess($"new sln -n \"{ProjectName}\" -o \"{RepositoryPath}\"");

        public DotnetCoreSolutionWriter(DotnetCoreWriterSettings settings) : base(settings)
        {
        }
    }
}
