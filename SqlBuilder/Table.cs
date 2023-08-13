using System.Data;

namespace SqlBuilder
{
    public class Table<TEntity>
        where TEntity : ISqlTable
    {

        protected readonly IDbConnection _connection;

        public Table(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Create(TEntity entity)
        {

            var parameters = new List<Parameter>();

            var t = entity.GetType();
            var properties = t.GetProperties();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(entity, null);

                if (propertyValue is null)
                {
                    continue;
                }

                parameters.Add(new Parameter
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = propertyValue
                });
            }

            var values = string.Join(", ", parameters.Select(p =>
            {
                if (p.Type == typeof(string))
                {
                    return $"'{p.Value}'";
                }

                return p.Value;

            }));

            var generatedSql = $"INSERT INTO {typeof(TEntity).Name} VALUES ({values})";

            using var command = _connection.CreateCommand();
            command.CommandText = generatedSql;
            command.ExecuteNonQuery();
            command.Dispose();
        }
    }
}