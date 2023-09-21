using SqlBuilder;

namespace App
{
    /// <summary>
    /// Representa un contexto de base de datos que se conecta a una base de datos SQLite y proporciona acceso a tablas relacionadas.
    /// </summary>
    public class Database : DatabaseContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase Database con la cadena de conexión especificada.
        /// </summary>
        public Database() : base(@"Data Source=test.db")
        {
            Toys = new Table<Toy>(ConnectionString);
        }

        /// <summary>
        /// Obtiene o establece una instancia de la clase Table que representa la tabla "Toys" en la base de datos.
        /// </summary>
        public Table<Toy> Toys { get; set; }
    }
}
