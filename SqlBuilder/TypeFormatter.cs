namespace SqlBuilder;

public static class TypeFormatter
{
    public static string Format(Type type, object? value)
    {
        if (value == null)
        {
            return "NULL";
        }

        if (type == typeof(string))
        {
            return $"'{value}'";
        }

        if (type == typeof(float) || type == typeof(int))
        {
            return value?.ToString() ?? string.Empty;
        }

        if (type == typeof(DateTime))
        {
            var dateTimeValue = (DateTime)value;
            return dateTimeValue.ToString("o");
        }

        throw new NotSupportedException($"Type {type} is not supported");
    }
}