using System.Data;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    public class DatabaseContext
    {
        protected readonly SqliteConnection Connection;

        protected DatabaseContext(string connectionString)
        {
            Connection = new SqlConnectionFactory(connectionString).GetConnection();
        }
    }
}