using System.Linq.Expressions;
using Microsoft.Data.Sqlite;

namespace SqlBuilder;

public class SqlExpressionVisitor : ExpressionVisitor
{
    private string _whereClause = "";
    private readonly IList<SqliteParameter> _sqlParameters = new List<SqliteParameter>();

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

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Visit(node.Left);

        if (!_symbolsTable.ContainsKey(node.NodeType))
        {
            throw new NotSupportedException($"Operator {node.NodeType.ToString()} not supported");
        }

        _whereClause += _symbolsTable[node.NodeType];

        Visit(node.Right);

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ParameterExpression)
        {
            // TODO: Check columns names
            _whereClause += node.Member.Name;
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        // TODO: Check types
        var parameterName = $"$param{_sqlParameters.Count}";
        var parameterValue = TypeFormatter.Format(node.Type, node.Value);
        _sqlParameters.Add(new SqliteParameter(parameterName, parameterValue));
        _whereClause += parameterName;
        return node;
    }
}