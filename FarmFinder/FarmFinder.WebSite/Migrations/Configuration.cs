using System.Collections.Generic;
using System.Data.Entity.Migrations;
using CH.Tutteli.FarmFinder.Website.Models;

namespace CH.Tutteli.FarmFinder.Website.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //context.Farms.AddOrUpdate(f => f.Name,
            //    new Farm
            //    {
            //        Name = "Ebelsberghof",
            //        Latitude = 48.256453,
            //        Longitude = 14.351921,
            //        Address = "Neufelderstrasse 3",
            //        City = "Linz",
            //        Zip = "4033",
            //        Email = "info@ebelsbergerhof.at",
            //        PhoneNumber = "+43 660 123 45 67",
            //        Website = "http://ebelsbergerhof.at",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                Description = "Die Tomaten werden im Gewächshaus angebaut und von Hand gepflückt.",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = false
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Müllers Bio-Rinder-Range",
            //        Latitude = 48.359072,
            //        Longitude = 14.362907,
            //        Address = "Oberkulm 1",
            //        Zip = "4203",
            //        City = "Altenberg bei Linz",
            //        PhoneNumber = "+43 123 456 78 90",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Porree",
            //                Description = "Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Frisch von Urfahr",
            //        Latitude = 48.313752,
            //        Longitude = 14.269738,
            //        Address = "Am grünen Hang",
            //        Zip = "4020",
            //        City = "Linz",
            //        PhoneNumber = "+43 578 963 85 21",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Porree",
            //                Description = "Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Romans Bauernhof",
            //        Latitude = 48.302466,
            //        Longitude = 14.402733,
            //        Address = "Holzwinden",
            //        Zip = "4221",
            //        City = "Steyregg",
            //        Email = "romans-bauernhof@gmx.net",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Porree",
            //                InStock = false
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Mayrs Hof",
            //        Latitude = 48.317164,
            //        Longitude = 14.468286,
            //        Address = "Grünbachstrasse",
            //        Zip = "4221",
            //        City = "Steyregg",
            //        PhoneNumber = "+43 741 852 96 30",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Lauch",
            //                Description = "Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Pöllerbachhof",
            //        Latitude = 48.435684,
            //        Longitude = 14.286400,
            //        Address = "Davidschlag 43",
            //        Zip = "4202",
            //        City = "Kirchschlag bei Linz",
            //        Website = "http://poellerbach.at",
            //        PhoneNumber = "+43 789 456 12 23",
            //        Email = "bestellung@poellerbach.at",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Gesundes Essen",
            //        Latitude = 48.454379,
            //        Longitude = 14.096768,
            //        Address = "Am Emelberg",
            //        Zip = "4174",
            //        City = "Niederwaldkirchen",
            //        PhoneNumber = "+43 600 100 10 10",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "frische Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Görlitzerhof",
            //        Latitude = 48.451470,
            //        Longitude = 14.120414,
            //        Address = "Görlitzer",
            //        Zip = "4174",
            //        City = "Niederwaldkirchen",
            //        PhoneNumber = "+43 600 200 20 20",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Rispentomaten",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Biohof Eidenberg",
            //        Latitude = 48.387631,
            //        Longitude = 14.223175,
            //        Address = "Eidenberg",
            //        Zip = "4201",
            //        City = "Eidenberg",
            //        Website = "http://bio-eidenberg.at",
            //        Email = "info@eidenberg.at",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Theos Bauernhof",
            //        Latitude = 48.366788,
            //        Longitude = 14.250984,
            //        Address = "Hametner 12a",
            //        Zip = "4040",
            //        City = "Lichtenberg",
            //        PhoneNumber = "+43 578 945 12 12",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //               new Product
            //            {
            //                Name = "Lauch",
            //                Description = "Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.",
            //                InStock = true
            //            },
            //        }
            //    },
            //    new Farm
            //    {
            //        Name = "Hubls Hof",
            //        Latitude = 48.277732,
            //        Longitude = 14.537401,
            //        Address = "Zeinersdorf",
            //        Zip = "4312",
            //        City = "Ried in der Riedmark",
            //        PhoneNumber = "+43 120 200 12 12",
            //        Products = new List<Product>
            //        {
            //            new Product
            //            {
            //                Name = "Tomaten",
            //                InStock = false
            //            },
            //            new Product
            //            {
            //                Name = "Salat Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Festkochende Kartoffeln",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Gurken",
            //                InStock = true
            //            },
            //            new Product
            //            {
            //                Name = "Karotten",
            //                InStock = true
            //            },
            //               new Product
            //            {
            //                Name = "Lauch",
            //                Description = "Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.",
            //                InStock = true
            //            },
            //        }
            //    });
        }
    }
}