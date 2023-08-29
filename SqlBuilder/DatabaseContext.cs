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
    }
}