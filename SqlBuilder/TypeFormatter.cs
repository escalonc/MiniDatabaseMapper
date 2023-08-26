
public class TypeFormatter {

    public static string Format(Object? value) {

        if (value == null) {
            return "NULL";
        }

        if (value.GetType() == typeof(string))
        {
            return $"'{value}'";
        }
        else if (value.GetType() == typeof(float) || value.GetType() == typeof(int))
        {
            return value.ToString();
        } else if (value.GetType() == typeof(DateTime))
        {
            DateTime dateTimeValue = (DateTime)value;
            return dateTimeValue.ToString("o"); // Formato ISO 8601
        } else
        {
            throw new NotSupportedException($"Type {value.GetType().ToString()} is not supported");
        }
    }
}
