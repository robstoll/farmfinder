using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.Tutteli.FarmFinder.Website.Models;

namespace FarmFinder.Seeding
{
    public class Seeder
    {
        public static void Main(string[] args)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.ExecuteSqlCommand("truncate Table [dbo].Products");
                db.Database.ExecuteSqlCommand("DELETE FROM [dbo].Farms");
                db.Database.ExecuteSqlCommand(@"
                SET IDENTITY_INSERT [dbo].Farms ON                
                insert into [dbo].Farms (FarmId, Name, Latitude, Longitude, Address, Zip, City,Email,Website,PhoneNumber, UpdateDateTime, IndexDateTime, DeleteWhenRemovedFromIndex) VALUES 
                (1,'Ebelsberghof',48.256453,14.351921,'Neufelderstrasse 3','Linz','4032','info@ebelsbergerhof.at','http://ebelsbergerhof.at','+43 660 123 45 67',GETDATE(),'0001-01-01T00:00:00',0),
                (2,'Müllers Bio-Rinder-Range',48.359072,14.362907,'Oberkulm 1','Altenberg bei Linz','4203',NULL,NULL,'+43 123 456 78 90',GETDATE(),'0001-01-01T00:00:00',0),
                (3,'Frisch von Urfahr',48.313752,14.269738,'Am grünen Hang','Linz','4020',NULL,NULL,'+43 578 963 85 21',GETDATE(),'0001-01-01T00:00:00',0),
                (4,'Romans Bauernhof',48.302466,14.402733,'Holzwinden','Steyregg','4221','romans-bauernhof@gmx.net',NULL,NULL,GETDATE(),'0001-01-01T00:00:00',0),
                (5,'Mayrs Hof',48.317164,14.468286,'Grünbachstrasse','Steyregg','4221',NULL,NULL,'+43 741 852 96 30',GETDATE(),'0001-01-01T00:00:00',0),
                (6,'Pöllerbachhof',48.435684,14.2864,'Davidschlag 43','Kirchschlag bei Linz','4202','bestellung@poellerbach.at','http://poellerbach.at','+43 789 456 12 23',GETDATE(),'0001-01-01T00:00:00',0),
                (7,'Gesundes Essen',48.454379,14.096768,'Am Emelberg','Niederwaldkirchen','4174',NULL,NULL,'+43 600 100 10 10',GETDATE(),'0001-01-01T00:00:00',0),
                (8,'Görlitzerhof',48.45147,14.120414,'Görlitzer','Niederwaldkirchen','4174',NULL,NULL,'+43 600 200 20 20',GETDATE(),'0001-01-01T00:00:00',0),
                (9,'Biohof Eidenberg',48.387631,14.223175,'Eidenberg','Eidenberg','4201','info@eidenberg.at','http://bio-eidenberg.at',NULL,GETDATE(),'0001-01-01T00:00:00',0),
                (10,'Theos Bauernhof',48.366788,14.250984,'Hametner 12a','Lichtenberg','4040',NULL,NULL,'+43 578 945 12 12',GETDATE(),'0001-01-01T00:00:00',0),
                (11,'Hubls Hof',48.277732,14.537401,'Zeinersdorf','Ried in der Riedmark','4312',NULL,NULL,'+43 120 200 12 12',GETDATE(),'0001-01-01T00:00:00',0);
                SET IDENTITY_INSERT [dbo].Farms OFF
                ");
                db.Database.ExecuteSqlCommand(@"
                insert into [dbo].Products (FarmRefId, InStock, Name, Description) VALUES
                (1,1,'Tomaten','Die Tomaten werden im Gewächshaus angebaut und von Hand gepflückt.'),
                (1,1,'Salat Kartoffeln',NULL),
                (1,1,'Festkochende Kartoffeln',NULL),
                (1,0,'Gurken',NULL),
                (1,0,'Karotten',NULL),
                (2,0,'Tomaten',NULL),
                (2,1,'Salat Kartoffeln',NULL),
                (2,1,'Festkochende Kartoffeln',NULL),
                (2,0,'Gurken',NULL),
                (2,0,'Karotten',NULL),
                (2,1,'Porree','Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.'),
                (3,0,'Tomaten',NULL),
                (3,0,'Gurken',NULL),
                (3,1,'Karotten',NULL),
                (3,1,'Porree','Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.'),
                (4,0,'Tomaten',NULL),
                (4,1,'Salat Kartoffeln',NULL),
                (4,1,'Festkochende Kartoffeln',NULL),
                (4,0,'Gurken',NULL),
                (4,1,'Karotten',NULL),
                (4,0,'Porree',NULL),
                (5,1,'Tomaten',NULL),
                (5,1,'Kartoffeln',NULL),
                (5,0,'Sellerie',NULL),
                (5,0,'Karotten',NULL),
                (5,0,'Lauch','Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.'),
                (6,1,'Tomaten',NULL),
                (6,1,'Salat Kartoffeln',NULL),
                (6,0,'Festkochende Kartoffeln',NULL),
                (6,1,'Gurken',NULL),
                (6,1,'Karotten',NULL),
                (7,1,'frische Tomaten',NULL),
                (7,1,'Salat Kartoffeln',NULL),
                (7,1,'Kartoffeln',NULL),
                (7,1,'Gurken',NULL),
                (7,1,'Karotten',NULL),
                (8,1,'Rispentomaten',NULL),
                (8,1,'Salat Kartoffeln',NULL),
                (8,0,'Festkochende Kartoffeln',NULL),
                (8,0,'Gurken',NULL),
                (8,1,'Karotten',NULL),
                (9,1,'Tomaten',NULL),
                (9,1,'Salat Kartoffeln',NULL),
                (9,1,'Festkochende Kartoffeln',NULL),
                (9,0,'Gurken',NULL),
                (9,0,'Karotten',NULL),
                (10,1,'Tomaten',NULL),
                (10,1,'Salat Kartoffeln',NULL),
                (10,1,'Festkochende Kartoffeln',NULL),
                (10,1,'Gurken',NULL),
                (10,0,'Karotten',NULL),
                (10,1,'Lauch','Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.'),
                (11,1,'Tomaten',NULL),
                (11,1,'Salat Kartoffeln',NULL),
                (11,1,'Festkochende Kartoffeln',NULL),
                (11,1,'Gurken',NULL),
                (11,1,'Karotten',NULL),
                (11,1,'Lauch','Spanischer Lauch welche im Sommer angepflanzt und für den Winter eingelagert wurde.');");

               
                var lat = 45.0;
                var lng = 5.0;
                var id = 100;
                var random = new Random();

                var products = new[]
                {
                    "Tomaten",
                    "Gurken",
                    "Kartoffeln",
                    "Lauch",
                    "Zwiebeln",
                    "Karotten",
                    "Paprika",
                    "Weisskohl",
                    "Rotkohl",
                    "Kohlsprossen",
                    "Sellerie",
                    "Rettich",
                    "Kürbis",
                    "Eisbergsalat",
                    "Kopfsalat",
                    "Melanzani",
                    "Zucchini"
                };

                for (int i = 0; i < 10; ++i)
                {
                    var sbFarms = new StringBuilder("SET IDENTITY_INSERT [dbo].Farms ON; insert into [dbo].Farms (FarmId, Name, Latitude, Longitude, Address, Zip, City, UpdateDateTime, IndexDateTime, DeleteWhenRemovedFromIndex) VALUES\n");
                    var sbProducts = new StringBuilder("insert into [dbo].Products (FarmRefId, InStock, Name) VALUES\n");
                    
                    for (int j = 0; j < 100; ++j)
                    {
                        if (j != 0)
                        {
                            sbFarms.AppendLine(",");
                        }
                        sbFarms.Append("(")
                            .Append(id).Append(",")
                            .Append("'Farm ").Append(id).Append("',")
                            .Append(lat).Append(",")
                            .Append(lng).Append(",")
                            .Append("'Adresse ").Append(id).Append("',")
                            .Append("'1234',")
                            .Append("'Ort ").Append(id).Append("',")
                            .Append("GETDATE(),")
                            .Append("'0001-01-01T00:00:00',")
                            .Append("0")
                            .Append(")");

                        var numberOfProducts = random.Next(1, 11);
                        for (int k = 0; k < numberOfProducts; ++k)
                        {
                            if (j != 0 || k != 0)
                            {
                                sbProducts.AppendLine(",");
                            }
                            sbProducts.Append("(")
                                .Append(id).Append(",")
                                .Append(random.Next(0,2)).Append(",")
                                .Append("'").Append(products[random.Next(0,products.Length)]).Append("'")
                                .Append(")");
                        }
                        
                        ++id;
                        lat += 0.002;

                    }
                    lng += 0.01;
                    lat = 45.0;
                    sbFarms.Append("SET IDENTITY_INSERT [dbo].Farms OFF");
                    db.Database.ExecuteSqlCommand(sbFarms.ToString());
                    db.Database.ExecuteSqlCommand(sbProducts.ToString());
                }
            }
        }
    }
}
