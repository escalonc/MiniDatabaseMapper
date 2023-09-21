using SqlBuilder;

namespace App
{
    /// <summary>
    /// Representa una entidad "Toy" que se puede asociar a una tabla en una base de datos SQLite.
    /// </summary>
    public class Toy : ISqlTable
    {
        /// <summary>
        /// Obtiene o establece el identificador único del juguete.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del juguete.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del juguete.
        /// </summary>
        public int Price { get; set; }
    }
}
