using SqlBuilder;

namespace App
{
    public class Database : DatabaseContext
    {
        public Database() : base(@"Data Source=test.db")
        {
            Toys = new Table<Toy>(connection);
        }

        public Table<Toy> Toys { get; set; }
    }
}