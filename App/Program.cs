using App;

Console.WriteLine("Hello, World!");

var db = new Database();

db.Toys.Create(new Toy { Name = "Carrito", Price = 10 });
// db.Toys.Delete(x => x.Name == "Carrito");
// var ids = new int[] { 1, 3, 4, 5 };
// db.Toys.Delete(x => ids.Contains(x.Id));