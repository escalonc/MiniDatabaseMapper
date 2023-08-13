// See https://aka.ms/new-console-template for more information
using App;

Console.WriteLine("Hello, World!");

var db = new Database();

db.Toys.Create(new Toy { Name = "Teddy Bear", Price = 10 });

