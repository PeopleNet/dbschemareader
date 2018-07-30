using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader.ProviderSchemaReaders.ConnectionContext;

namespace DatabaseSchemaReader.ProviderSchemaReaders.Databases.PostgreSql
{
    internal class ViewColumns : SqlExecuter<DatabaseColumn>
    {
        private readonly string _viewName;

        public ViewColumns(string owner, string viewName)
        {
            _viewName = viewName;
            Owner = owner;
            Sql = @"select c.table_schema,
c.table_name, 
column_name, 
ordinal_position, 
column_default, 
is_nullable, 
data_type, 
character_maximum_length, 
numeric_precision, 
numeric_scale
from information_schema.columns c
join information_schema.views v 
 ON c.table_schema = v.table_schema and 
    c.table_name = v.table_name
where 
    (c.table_schema = @Owner or (@Owner is null)) and 
    (c.table_name = @TableName or (@TableName is null))
 order by 
    c.table_schema, c.table_name, ordinal_position";
        }

        public IList<DatabaseColumn> Execute(IConnectionAdapter connectionAdapter)
        {
            ExecuteDbReader(connectionAdapter);
            return Result;
        }

        protected override void AddParameters(DbCommand command)
        {
            AddDbParameter(command, "@Owner", Owner);
            AddDbParameter(command, "@TableName", _viewName);
        }

        protected override void Mapper(IDataRecord record)
        {
            var col = new DatabaseColumn
            {
                SchemaOwner = record.GetString("table_schema"),
                TableName = record.GetString("table_name"),
                Name = record.GetString("column_name"),
                Ordinal = record.GetInt("ordinal_position"),
                Nullable = record.GetBoolean("is_nullable"),
                DefaultValue = record.GetString("column_default"),
                DbDataType = record.GetString("data_type"),
                Length = record.GetNullableInt("character_maximum_length"),
                Precision = record.GetNullableInt("numeric_precision"),
                Scale = record.GetNullableInt("numeric_scale"),
                //DateTimePrecision = record.GetNullableInt("DATETIME_PRECISION"),
            };
            Result.Add(col);
        }
    }
}
