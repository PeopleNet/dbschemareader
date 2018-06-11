namespace DatabaseSchemaReader.CodeGen
{
    using DatabaseSchemaReader.DataSchema;
    using McMaster.Extensions.CommandLineUtils;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    public abstract class DotnetCoreWriter : IWriter
    {
        private readonly DotnetCoreWriterSettings settings;
        protected string RepositoryName;
        protected string RepositoryPath;
        protected string ProjectName;

        public DotnetCoreWriter(DotnetCoreWriterSettings settings)
        {
            this.settings = settings;
            RepositoryName = settings.RepositoryNameTemplate.Replace("<tablename>", settings.Table.Name.ToLower());
            RepositoryPath = Path.Combine(settings.OutputDirectory.FullName, RepositoryName);
            ProjectName = settings.ProjectNameTemplate.Replace("<tablename>", settings.Table.Name);
        }

        public abstract Task Execute();

        protected Process GetDotNetProcess()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = DotNetExe.FullPathOrDefault();
            return p;
        }

        protected async Task StartDotNetProcess(string arguments)
        {
            var p = GetDotNetProcess();
            p.StartInfo.Arguments = arguments;
            await Task.Run(() =>
            {
                p.Start();
                p.WaitForExit();
                p.Close();
            });
        }
    }
}
