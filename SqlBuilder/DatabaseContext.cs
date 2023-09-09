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
            var type = typeof(ISqlTable);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var tableType in types)
            {
                var tableName = tableType.Name;

                var columnProperties = tableType.GetProperties();

                foreach (var property in columnProperties)
                {
                        // check types and build sql fields
                }
                
                // concatenate fields with create statement
                
                //execute query
            }
        }
        
    }
}