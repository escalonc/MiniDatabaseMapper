using System.Data;

namespace SqlBuilder
{
    public class DatabaseContext
    {
        protected readonly IDbConnection Connection;

        protected DatabaseContext(string connectionString)
        {
            Connection = new SqlConnectionFactory(connectionString).GetConnection();
        }
    }
}