using System.Data;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    public class DatabaseContext
    {
        protected readonly string ConnectionString;

        protected DatabaseContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Initialize()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var type = typeof(ISqlTable);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p != type);

                foreach (var tableType in types)
                {
                    var tableName = tableType.Name;

                    var columnProperties = tableType.GetProperties();
                    var columns = new List<string>();

                    foreach (var property in columnProperties)
                    {
                        var columnName = property.Name;
                        var columnType = GetSqlTypeForProperty(property.PropertyType);

                        // check types and build sql fields
                        columns.Add($"{columnName} {columnType}");
                    }

                     // concatenate fields with create statement
                    var createTableSql = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(", ", columns)})";

                    //execute query
                    //using (var command = new SqliteCommand(createTableSql, connection))
                    //{
                    //  command.ExecuteNonQuery();
                    //}
                }
            }
        }

        private string GetSqlTypeForProperty(Type propertyType)
        {
            if (propertyType == typeof(string))
            {
                return "TEXT";
            }
            else if (propertyType == typeof(int))
            {
                return "INTEGER";
            }
            else if (propertyType == typeof(decimal))
            {
                return "NUMERIC";
            }
            else if (propertyType == typeof(bool))
            {
                return "BOOLEAN";
            }
            else
            {
                throw new ArgumentException($"Unsupported property type: {propertyType.Name}");
            }
        }
        
    }
}