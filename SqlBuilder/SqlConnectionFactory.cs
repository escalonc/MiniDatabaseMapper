using System.Data;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    /// <summary>
    /// Una fábrica de conexiones SQLite que proporciona conexiones abiertas a la base de datos.
    /// </summary>
    public class SqlConnectionFactory
    {
        private readonly SqliteConnection _connection;

        /// <summary>
        /// Inicializa una nueva instancia de la clase SqlConnectionFactory con la cadena de conexión especificada.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión a la base de datos SQLite.</param>
        public SqlConnectionFactory(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        /// <summary>
        /// Obtiene una conexión abierta a la base de datos SQLite.
        /// </summary>
        /// <returns>Una instancia de SqliteConnection abierta y lista para su uso.</returns>
        public SqliteConnection GetConnection() => _connection;
    }
}
