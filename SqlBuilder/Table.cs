using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace SqlBuilder
{
    /// <summary>
    /// Representa una tabla en una base de datos SQLite y proporciona métodos para operar en ella.
    /// </summary>
    /// <typeparam name="TEntity">El tipo de entidad que se asocia a la tabla.</typeparam>
    public class Table<TEntity>
        where TEntity : ISqlTable, new()
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly PropertyInfo[] _properties;

        /// <summary>
        /// Inicializa una nueva instancia de la clase Table con la cadena de conexión especificada.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión a la base de datos SQLite.</param>
        public Table(string connectionString)
        {
            _tableName = typeof(TEntity).Name;
            _properties = typeof(TEntity).GetProperties();
            _connectionString = connectionString;
        }

        /// <summary>
        /// Crea un nuevo registro en la tabla utilizando la entidad especificada.
        /// </summary>
        /// <param name="entity">La entidad que se va a agregar a la tabla.</param>
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

        /// <summary>
        /// Elimina registros de la tabla que cumplan con un predicado especificado.
        /// </summary>
        /// <param name="predicate">El predicado que define qué registros se eliminarán.</param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlVisitor = new SqlExpressionVisitor();
            var result = sqlVisitor.Translate(predicate.Body);
            var sql = $"DELETE FROM {_tableName} WHERE {result.whereClause}";
            ExecuteCommand(sql, result.sqliteParameters);
        }

        /// <summary>
        /// Busca y recupera registros de la tabla que cumplan con un predicado especificado.
        /// </summary>
        /// <param name="predicate">El predicado que define qué registros se buscarán y recuperarán.</param>
        /// <returns>Una colección de entidades que cumplen con el predicado.</returns>
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

                    var value = record[property.Name].ToString();

                    if (value == null)
                    {
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(entity, Convert.ToInt32(value));
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(decimal))
                    {
                        property.SetValue(entity, Convert.ToDecimal(value));
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(float))
                    {
                        property.SetValue(entity, Convert.ToSingle(value));
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(entity, Convert.ToDouble(value));
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(entity, DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None));
                        continue;
                    }
                    
                    if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(entity, Convert.ToBoolean(value));
                        continue;
                    }

                    property.SetValue(entity, record[property.Name]);
                }

                entities.Add(entity);
            }

            return entities;
        }

        /// <summary>
        /// Actualiza registros en la tabla que cumplan con un predicado especificado utilizando los datos de una entidad de actualización.
        /// </summary>
        /// <param name="entityUpdate">La entidad que contiene los datos de actualización.</param>
        /// <param name="predicate">El predicado que define qué registros se actualizarán.</param>
        public void  Update(object entityUpdate, Expression<Func<TEntity, bool>> predicate)
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


        /// <summary>
        /// Ejecuta una instrucción SQL en la base de datos utilizando parámetros.
        /// </summary>
        /// <param name="sql">La instrucción SQL a ejecutar.</param>
        /// <param name="parameters">Los parámetros de la instrucción SQL.</param>
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

        /// <summary>
        /// Ejecuta una consulta SQL en la base de datos utilizando parámetros y devuelve un conjunto de resultados.
        /// </summary>
        /// <param name="sql">La consulta SQL a ejecutar.</param>
        /// <param name="parameters">Los parámetros de la consulta SQL.</param>
        /// <returns>Una colección de registros de datos resultantes.</returns>
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