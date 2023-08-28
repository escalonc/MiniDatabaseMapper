using App;

Console.WriteLine("Hello, World!");

var db = new Database();

// db.Toys.Create(new Toy { Name = "test4", Price = 444 });
// db.Toys.Delete(x => x.Id == 0 && x.Name == "test3");
// var ids = new int[] { 1, 3, 4, 5 };
// db.Toys.Delete(x => ids.Contains(x.Id));
// db.Toys.Find(x => x.Id == 0);

db.Toys.Update(new
{
    Name = "test4",
    Price = 500
}, t => t.Id == 1);