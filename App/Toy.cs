using SqlBuilder;

namespace App
{
    public class Toy : ISqlTable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }
    }
}