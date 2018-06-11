using DatabaseSchemaReader.DataSchema;
using System.IO;
using System.Threading.Tasks;

namespace DatabaseSchemaReader.CodeGen
{
    public class DotnetCoreWebApiProjectWriter : DotnetCoreWriter
    {
        public DotnetCoreWebApiProjectWriter(DotnetCoreWriterSettings settings) : base(settings)
        {
        }

        public override Task Execute() => StartDotNetProcess($"new webapi -n {ProjectName} -o {Path.Combine(RepositoryPath, ProjectName)}");
    }
}
