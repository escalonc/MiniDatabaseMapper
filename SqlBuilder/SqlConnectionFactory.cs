using System.Data;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    public class SqlConnectionFactory
    {
        private readonly SqliteConnection _connection;

        public SqlConnectionFactory(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        public SqliteConnection GetConnection() => _connection;
    }
}