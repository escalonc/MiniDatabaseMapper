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

        public Table(IDbConnection connection)
        {
            _connection = connection;
            _tableName = typeof(TEntity).Name;
            _sqlVisitor = new SqlExpressionVisitor();
            _properties = typeof(TEntity).GetProperties();
        }

        public void Create(TEntity entity)
        {
            var parameters = (from property in _properties
                    let propertyValue = property.GetValue(entity, null)
                    where propertyValue is not null
                    select new Parameter { Name = property.Name, Type = property.PropertyType, Value = propertyValue })
                .ToList();


            var values = string.Join(", ", parameters.Select(p => (string)TypeFormatter.Format(p.Value)));

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
            
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            var reader = command.ExecuteReader();

            var data = Activator.CreateInstance(typeof(TEntity));

            var entity = new TEntity();

            while (reader.Read())
            {
                
            }
            
            return new List<TEntity>();
        }

        private void ExecuteCommand(string sql)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}