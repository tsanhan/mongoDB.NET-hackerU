using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace MongoDB.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            BsonClassMap.RegisterClassMap<NameModel>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            MongoCRUD db = new MongoCRUD("AddressBook");

            var recs = db.LoadRecords<NameModel>("Users");

            System.Console.WriteLine("done program");

            foreach (var rec in recs)
            {
                System.Console.WriteLine($"{rec.Id} {rec.FirstName} {rec.LastName}");

                System.Console.WriteLine();
            }




            Console.ReadLine();
        }
    }
}
