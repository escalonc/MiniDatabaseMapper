using System.Data;
using Microsoft.Data.Sqlite;

/// <summary>
/// Representa un contexto de base de datos que se encarga de inicializar las tablas
/// en una base de datos SQLite según las clases que implementan la interfaz ISqlTable.
/// </summary>
namespace SqlBuilder
{
    public class DatabaseContext
    {
        protected readonly string ConnectionString;

        /// <summary>
        /// Constructor de la clase DatabaseContext.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión a la base de datos SQLite.</param>
        protected DatabaseContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Inicializa las tablas en la base de datos según las clases que implementan la interfaz ISqlTable.
        /// </summary>
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

                // Concatena los campos con la instrucción CREATE TABLE
                var createTableSql = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(", ", columns)});";
                // Ejecuta la consulta
                using var command = new SqliteCommand(createTableSql, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene el tipo de dato SQL correspondiente al tipo de propiedad especificado.
        /// </summary>
        /// <param name="propertyType">El tipo de la propiedad.</param>
        /// <returns>El tipo de dato SQL.</returns>
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

            throw new ArgumentException($"Tipo de propiedad no admitido: {propertyType.Name}");
        }
    }
}
