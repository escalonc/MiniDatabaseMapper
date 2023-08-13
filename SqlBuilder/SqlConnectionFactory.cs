using System.Data;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    public class SqlConnectionFactory
    {
        private readonly IDbConnection _connection;

        public SqlConnectionFactory(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        public IDbConnection GetConnection() => _connection;
    }
}