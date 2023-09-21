using App;


var db = new Database();

db.Initialize();

//Create
db.Toys.Create(new Toy { Name = "Osito", Price = 444 });
db.Toys.Create(new Toy { Name = "Carrito", Price = 50 });
db.Toys.Create(new Toy { Name = "Muñeca", Price = 100 });
db.Toys.Create(new Toy { Name = "Pony", Price = 250 });
db.Toys.Create(new Toy { Name = "Peluche", Price = 60 });

//Read
var toys = db.Toys.Find(x => x.Id == 0);

foreach (var toy in toys)
 {
     Console.WriteLine($"Name: {toy.Name}, Price{toy.Price}");
 }

//Update
db.Toys.Update(new
{
    Price = 600
}, t => t.Price == 500);

//Delete
db.Toys.Delete(x => x.Price == 600);

Console.WriteLine("Hello, World!");
