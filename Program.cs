using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.NET
{
    class Program
    {
        static void Main(string[] args)
        {

            MongoCRUD db = new MongoCRUD("AddressBook");

            db.InserRecord("Users", new PersonModel { FirstName = "Victor", LastName = "Yampolsky" });
            System.Console.WriteLine("done program");
            Console.ReadLine();
        }
    }

    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }



    }

    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void InserRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);


        }



    }
}
