using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DatabaseSchemaReader.CodeGen.CodeFirst;
using DatabaseSchemaReader.CodeGen.NHibernate;
using DatabaseSchemaReader.CodeGen.Procedures;
using DatabaseSchemaReader.DataSchema;

namespace DatabaseSchemaReader.CodeGen
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A *simple* code generation
    /// </summary>
    public class CodeWriter : IWriter
    {
        private readonly DatabaseSchema schema;

        private string mappingPath;

        private MappingNamer mappingNamer;

        private readonly CodeWriterSettings codeWriterSettings;

        private readonly ProjectVersion projectVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="codeWriterSettings">The code writer settings.</param>
        public CodeWriter(DatabaseSchema schema, CodeWriterSettings codeWriterSettings)
        {
            this.schema = schema;
            this.codeWriterSettings = codeWriterSettings;


            var vs2010 = this.codeWriterSettings.WriteProjectFile;
            var vs2015 = this.codeWriterSettings.WriteProjectFileNet46;
            projectVersion = vs2015 ? ProjectVersion.Vs2015 : vs2010 ? ProjectVersion.Vs2010 : ProjectVersion.Vs2008;
            //cannot be .net 3.5
            if (IsCodeFirst() && projectVersion == ProjectVersion.Vs2008) projectVersion = ProjectVersion.Vs2015;

            PrepareSchemaNames.Prepare(schema, codeWriterSettings.Namer);
        }

        /// <summary>
        /// Uses the specified schema to write class files, NHibernate/EF CodeFirst mapping and a project file. Any existing files are overwritten. If not required, simply discard the mapping and project file. Use these classes as ViewModels in combination with the data access strategy of your choice.
        /// </summary>
        /// <param name="directory">The directory to write the files to. Will create a subdirectory called "mapping". The directory must exist- any files there will be overwritten.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="System.Security.SecurityException" />
        public async Task Execute()
        {
            if (codeWriterSettings.OutputDirectory == null)
            {
                throw new ArgumentNullException("directory");
            }

            if (!codeWriterSettings.OutputDirectory.Exists)
            {
                throw new InvalidOperationException("Directory does not exist: " + codeWriterSettings.OutputDirectory.FullName);
            }

            mappingNamer = new MappingNamer();

            foreach (var dataType in schema.DataTypes)
            {
                var dtw = new DataTypeWriter(dataType, codeWriterSettings);
                var txt = dtw.Write();
                if (string.IsNullOrEmpty(txt))
                {
                    continue;
                }

                WriteClassFile(codeWriterSettings.OutputDirectory, dataType.NetDataType, txt);
            }

            foreach (var table in schema.Tables)
            {
                if (FilterIneligible(table)) continue;
                var className = table.NetName;
                UpdateEntityNames(className, table.Name);

                var classWriter = new ClassWriter(table, codeWriterSettings);
                var classText = classWriter.Write();
                WriteClassFile(codeWriterSettings.OutputDirectory, className, classText);

                var repositoryInterfaceWriter = new RepositoryInterfaceWriter(table, codeWriterSettings);
                var interfaceText = repositoryInterfaceWriter.Write();
                WriteClassFile(codeWriterSettings.OutputDirectory, CodeWriterUtils.GetRepositoryInterfaceName(table), interfaceText);

                var repositoryImplementationWriter = new RepositoryImplementationWriter(
                    table,
                    codeWriterSettings,
                    codeWriterSettings.LogicalDeleteColumns);
                var implementationText = repositoryImplementationWriter.Write();
                WriteClassFile(codeWriterSettings.OutputDirectory, CodeWriterUtils.GetRepositoryImplementationName(table), implementationText);
            }

            if (codeWriterSettings.IncludeViews)
            {
                foreach (var view in schema.Views)
                {
                    var className = view.NetName;
                    UpdateEntityNames(className, view.Name);

                    var cw = new ClassWriter(view, codeWriterSettings);
                    var txt = cw.Write();

                    WriteClassFile(codeWriterSettings.OutputDirectory, className, txt);
                }
            }
        }

        private static string WriteClassFile(DirectoryInfo directory, string className, string txt)
        {
            var fileName = className + ".cs";
            var path = Path.Combine(directory.FullName, fileName);
            if (!directory.Exists) directory.Create();
            File.WriteAllText(path, txt);
            return fileName;
        }

        private bool IsCodeFirst()
        {
            return codeWriterSettings.CodeTarget == CodeTarget.PocoEntityCodeFirst
                   || codeWriterSettings.CodeTarget == CodeTarget.PocoEfCore;
        }

        private bool FilterIneligible(DatabaseTable table)
        {
            if (!IsCodeFirst()) return false;
            if (table.IsManyToManyTable() && codeWriterSettings.CodeTarget == CodeTarget.PocoEntityCodeFirst)
                return true;
            if (table.PrimaryKey == null)
                return true;
            if (table.Name.Equals("__MigrationHistory", StringComparison.OrdinalIgnoreCase)) //EF 6
                return true;
            if (table.Name.Equals("__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase)) //EF Core1
                return true;
            if (table.Name.Equals("EdmMetadata", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void UpdateEntityNames(string className, string tableName)
        {
            if (mappingNamer.EntityNames.Contains(className))
            {
                Debug.WriteLine("Name conflict! " + tableName + "=" + className);
            }
            else
            {
                mappingNamer.EntityNames.Add(className);
            }
        }
    }
}
