using System.Linq.Expressions;

namespace SqlBuilder;

public class SqlExpressionVisitor : ExpressionVisitor
{
    private string? _sql = "";

    public string? Translate(Expression expression)
    {
        Visit(expression);
        return _sql;
    }

    private readonly IDictionary<ExpressionType, string?> _symbolsTable = new Dictionary<ExpressionType, string?>()
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
            throw new NotSupportedException($"Operator {node.NodeType.ToString()} it's not supported");
        }

        _sql += _symbolsTable[node.NodeType];

        Visit(node.Right);

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ParameterExpression)
        {
            // Check columns names
            _sql += node.Member.Name;
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        // Check types
        _sql += TypeFormatter.Format(node.Type, node.Value);
        return node;
    }
}