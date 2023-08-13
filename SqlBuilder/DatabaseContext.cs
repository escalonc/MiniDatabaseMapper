using System.Data;

namespace SqlBuilder
{
    public class DatabaseContext
    {
        protected readonly IDbConnection connection;
        public DatabaseContext(string connectionString)
        {
            connection = new SqlConnectionFactory(connectionString).GetConnection();
        }
    }
}