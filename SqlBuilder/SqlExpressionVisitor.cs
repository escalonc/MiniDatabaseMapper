using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SqlBuilder
{
    /// <summary>
    /// Un visitante de expresiones que traduce una expresión LINQ en una cláusula WHERE SQL y parámetros SQLite.
    /// </summary>
    public class SqlExpressionVisitor : ExpressionVisitor
    {
        private string _whereClause = "";
        private readonly IList<SqliteParameter> _sqlParameters = new List<SqliteParameter>();

        /// <summary>
        /// Traduce una expresión LINQ en una cláusula WHERE SQL y parámetros SQLite.
        /// </summary>
        /// <param name="expression">La expresión LINQ a traducir.</param>
        /// <returns>
        /// Una tupla que contiene la cláusula WHERE SQL y una lista de parámetros SQLite.
        /// </returns>
        public (string whereClause, IList<SqliteParameter> sqliteParameters) Translate(Expression expression)
        {
            Visit(expression);
            return (_whereClause, _sqlParameters);
        }

        private readonly IDictionary<ExpressionType, string> _symbolsTable = new Dictionary<ExpressionType, string>()
        {
            { ExpressionType.And, " AND " },
            { ExpressionType.AndAlso, " AND " },
            { ExpressionType.Or, " OR " },
            { ExpressionType.OrElse, " OR " },
            { ExpressionType.Equal, " = " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThanOrEqual, " <= " },
            { ExpressionType.Not, " NOT " },
            { ExpressionType.NotEqual, " <> " },
        };

        /// <summary>
        /// Visita un nodo binario de una expresión.
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            if (!_symbolsTable.ContainsKey(node.NodeType))
            {
                throw new NotSupportedException($"Operador {node.NodeType.ToString()} no es compatible");
            }

            _whereClause += _symbolsTable[node.NodeType];

            Visit(node.Right);

            return node;
        }

        /// <summary>
        /// Visita un nodo de miembro (MemberExpression) de una expresión.
        /// </summary>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                // TODO: Verificar nombres de columnas
                _whereClause += node.Member.Name;
            }

            return node;
        }

        /// <summary>
        /// Visita un nodo constante (ConstantExpression) de una expresión.
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            // TODO: Verificar tipos
            var parameterName = $"$param{_sqlParameters.Count}";
            var parameterValue = TypeFormatter.Format(node.Type, node.Value);
            _sqlParameters.Add(new SqliteParameter(parameterName, parameterValue));
            _whereClause += parameterName;
            return node;
        }
    }
}
