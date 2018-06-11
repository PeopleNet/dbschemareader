using System.Threading.Tasks;

namespace DatabaseSchemaReader.CodeGen
{
    public interface IWriter
    {
        Task Execute();
    }
}
