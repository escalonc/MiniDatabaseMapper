/// <summary>
/// Interfaz que representa una entidad de tabla SQL básica con una propiedad "Id" de tipo entero.
/// </summary>
namespace SqlBuilder
{
    public interface ISqlTable
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la entidad en la tabla SQL.
        /// </summary>
        int Id { get; set; }
    }
}
