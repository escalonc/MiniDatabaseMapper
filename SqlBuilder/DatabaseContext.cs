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
            using var connection = new SqlConnectionFactory(ConnectionString).GetConnection();
            var type = typeof(ISqlTable);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != type);

            foreach (var tableType in types)
            {
                var tableName = tableType.Name;

                var columnProperties = tableType.GetProperties();
                var columns = (from property in columnProperties
                    let columnName = property.Name
                    let columnType = GetSqlTypeForProperty(property.PropertyType)
                    select $"{columnName} {columnType}").ToList();

                // concatenate fields with create statement
                var createTableSql = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(", ", columns)});";
                //execute query
                using var command = new SqliteCommand(createTableSql, connection);
                command.ExecuteNonQuery();
            }
        }

        private static string GetSqlTypeForProperty(Type propertyType)
        {
            if (propertyType == typeof(string))
            {
                return "TEXT";
            }

            if (propertyType == typeof(int))
            {
                return "INTEGER";
            }

            if (propertyType == typeof(decimal))
            {
                return "NUMERIC";
            }

            if (propertyType == typeof(bool))
            {
                return "BOOLEAN";
            }

            throw new ArgumentException($"Unsupported property type: {propertyType.Name}");
        }
    }
}