using App;


var db = new Database();

db.Initialize();

// db.Toys.Create(new Toy { Name = "Osito", Price = 444 });
db.Toys.Delete(x => x.Price == 600);
var toys = db.Toys.Find(x => x.Id == 0);

// foreach (var toy in toys)
// {
//     Console.WriteLine($"Name: {toy.Name}, Price{toy.Price}");
// }
//
db.Toys.Update(new
{
    Price = 600
}, t => t.Price == 500);
Console.WriteLine("Hello, World!");
