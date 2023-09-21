namespace SqlBuilder
{
    /// <summary>
    /// Proporciona métodos para formatear valores de diferentes tipos en representaciones de cadena para su uso en consultas SQL.
    /// </summary>
    public static class TypeFormatter
    {
        /// <summary>
        /// Formatea un valor de un tipo específico en una representación de cadena adecuada para su uso en consultas SQL.
        /// </summary>
        /// <param name="type">El tipo de datos del valor.</param>
        /// <param name="value">El valor que se formateará como cadena.</param>
        /// <returns>Una representación de cadena formateada del valor.</returns>
        public static string Format(Type type, object? value)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (type == typeof(float) || type == typeof(int) || type == typeof(string) || type == typeof(decimal) || type == typeof(bool))
            {
                return value?.ToString() ?? string.Empty;
            }

            if (type == typeof(DateTime))
            {
                var dateTimeValue = (DateTime)value;
                return dateTimeValue.ToString("o");
            }

            throw new NotSupportedException($"El tipo {type} no es compatible");
        }
    }
}
