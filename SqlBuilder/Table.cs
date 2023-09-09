using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    public class Table<TEntity>
        where TEntity : ISqlTable, new()
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly PropertyInfo[] _properties;

        public Table(string connectionString)
        {
            _tableName = typeof(TEntity).Name;
            _properties = typeof(TEntity).GetProperties();
            _connectionString = connectionString;
        }

        public void Create(TEntity entity)
        {
            var parameters = _properties
                .Select(p => new { FieldName = p.Name, Type = p.PropertyType, Value = p.GetValue(entity, null) })
                .Where(p => p.Value is not null)
                .Select((p, index) => new Parameter
                    { FieldName = p.FieldName, Name = $"$param{index}", Type = p.Type, Value = p.Value });

            var parameterNames = new List<string>();
            var sqlParameters = new List<SqliteParameter>();

            foreach (var parameter in parameters)
            {
                parameterNames.Add(parameter.Name);
                sqlParameters.Add(new SqliteParameter(parameter.Name,
                    TypeFormatter.Format(parameter.Type, parameter.Value)));
            }

            var sql = $"INSERT INTO {_tableName} VALUES ({string.Join(", ", parameterNames)})";
            ExecuteCommand(sql, sqlParameters);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlVisitor = new SqlExpressionVisitor();
            var result = sqlVisitor.Translate(predicate.Body);
            var sql = $"DELETE FROM {_tableName} WHERE {result.whereClause}";
            ExecuteCommand(sql, result.sqliteParameters);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlVisitor = new SqlExpressionVisitor();
            var result = sqlVisitor.Translate(predicate.Body);
            var sql = $"SELECT * FROM {_tableName} WHERE {result.whereClause}";

            var records = ExecuteReader(sql, result.sqliteParameters);

            var entities = new List<TEntity>();

            foreach (var record in records)
            {
                var entity = new TEntity();

                foreach (var property in _properties)
                {
                    // TODO: change with type converter function
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(entity, Convert.ToInt32(record[property.Name]));
                        continue;
                    }

                    property.SetValue(entity, record[property.Name]);
                }

                entities.Add(entity);
            }

            return entities;
        }

        public void Update(object entityUpdate, Expression<Func<TEntity, bool>> predicate)
        {
            var updateProperties = entityUpdate.GetType().GetProperties();
            var updatePropertyNames = updateProperties.Select(p => p.Name);
            var entityPropertyNames = _properties.Select(p => p.Name);

            var notMatchingPropertyNames = updatePropertyNames.Except(entityPropertyNames).ToList();

            if (notMatchingPropertyNames.Any())
            {
                throw new ArgumentException(
                    $"argument(s): [{string.Join(", ", notMatchingPropertyNames)}] to update does not match the expected field(s)");
            }

            var sqlVisitor = new SqlExpressionVisitor();
            var result = sqlVisitor.Translate(predicate.Body);

            var length = result.sqliteParameters.Count;

            var parameters = updateProperties.Select((p, index) => new Parameter
            {
                Name = $"$param{length + index}",
                Type = p.PropertyType,
                Value = p.GetValue(entityUpdate, null),
                FieldName = p.Name
            }).ToList();

            var sqlParameters = result.sqliteParameters.ToList();
            sqlParameters.AddRange(parameters.Select(p => new SqliteParameter(p.Name, p.Value)));

            var fields = string.Join(", ", parameters.Select(p => $"{p.FieldName} = {p.Name}"));

            var sql = $"UPDATE {_tableName} SET {fields} WHERE {result.whereClause}";

            ExecuteCommand(sql, sqlParameters);
        }


        private void ExecuteCommand(string sql, IList<SqliteParameter> parameters)
        {
            using var connection = new SqlConnectionFactory(_connectionString).GetConnection();
            Console.WriteLine($"Generated sql: {sql}");
            Console.WriteLine(
                $"Parameters: {string.Join(", ", parameters.Select(p => $"{p.ParameterName}: {p.Value}").ToList())}"
            );
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }

        private IEnumerable<IDataRecord> ExecuteReader(string sql, IList<SqliteParameter> parameters)
        {
            using var connection = new SqlConnectionFactory(_connectionString).GetConnection();
            Console.WriteLine($"Generated sql: {sql}");
            Console.WriteLine(
                $"Parameters: {string.Join(", ", parameters.Select(p => $"{p.ParameterName}: {p.Value}").ToList())}"
            );
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                yield return reader;
            }
        }
    }
}