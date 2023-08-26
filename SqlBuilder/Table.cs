using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBuilder
{
    public class Table<TEntity>
        where TEntity : ISqlTable, new()
    {
        private readonly IDbConnection _connection;
        private readonly string _tableName;
        private readonly SqlExpressionVisitor _sqlVisitor;
        private readonly PropertyInfo[] _properties;
        private readonly Type _type;

        public Table(IDbConnection connection)
        {
            _connection = connection;
            _tableName = typeof(TEntity).Name;
            _sqlVisitor = new SqlExpressionVisitor();
            _type = typeof(TEntity);
            _properties = _type.GetProperties();
        }
        
        public void Create(TEntity entity)
        {
            var parameters = (from property in _properties
                    let propertyValue = property.GetValue(entity, null)
                    where propertyValue is not null
                    select new Parameter { Name = property.Name, Type = property.PropertyType, Value = propertyValue })
                .ToList();


            var values = string.Join(", ", parameters.Select(p => p.Type == typeof(string) ? $"'{p.Value}'" : p.Value));

            var sql = $"INSERT INTO {_tableName} VALUES ({values})";
            ExecuteCommand(sql);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var whereSql = _sqlVisitor.Translate(predicate.Body);
            var sql = $"DELETE FROM {_tableName} WHERE {whereSql}";
            ExecuteCommand(sql);
        }

        public IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var whereSql = _sqlVisitor.Translate(predicate.Body);
            var sql = $"SELECT * FROM {_tableName} WHERE {whereSql}";

            var reader = ExecuteReader(sql);
            
            var entities = new List<TEntity>();

            while (reader.Read())
            {
                var entity = new TEntity();

                foreach (var property in _properties)
                {
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(entity, Convert.ToInt32(reader[property.Name]));
                        continue;
                    }

                    property.SetValue(entity, reader[property.Name]);
                }

                entities.Add(entity);
            }

            return entities;
        }

        private void ExecuteCommand(string sql)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            Console.WriteLine($"Generated sql: {sql}");
            command.ExecuteNonQuery();
        }

        private IDataReader ExecuteReader(string sql)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            Console.WriteLine($"Generated sql: {sql}");
            return command.ExecuteReader();
        }
    }
}