/// <summary>
/// Representa un parámetro utilizado en consultas SQL generadas por SqlBuilder.
/// </summary>
namespace SqlBuilder
{
    internal class Parameter
    {
        /// <summary>
        /// Obtiene o establece el nombre del campo o columna asociado al parámetro.
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// Obtiene o establece el nombre del parámetro.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de datos del parámetro.
        /// </summary>
        public required Type Type { get; set; }

        /// <summary>
        /// Obtiene o establece el valor del parámetro.
        /// </summary>
        public required object? Value { get; set; }
    }
}
