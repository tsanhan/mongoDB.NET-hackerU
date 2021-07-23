using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.NET
{
    class Program
    {
        static void Main(string[] args)
        {

            MongoCRUD db = new MongoCRUD("AddressBook");
            // PersonModel person = new PersonModel
            // {
            //     FirstName = "VicVic",
            //     LastName = "YamYam",
            //     PrimaryAddress = new AddressModel
            //     {
            //         city = "JLM",
            //         State = "ISL",
            //         StreetAddress = "Arie Ben Eliezer",
            //         ZipCode = "98321"
            //     }
            // };


            // db.InserRecord("Users", person);
            // System.Console.WriteLine("done program");

            // foreach (var rec in recs)
            // {
            //     System.Console.WriteLine($"{rec.Id} {rec.FirstName} {rec.LastName}");
            //     if (rec.PrimaryAddress != null)
            //     {
            //         System.Console.WriteLine(rec.PrimaryAddress.city);
            //     }
            //     System.Console.WriteLine();
            // }


            var oneRec = db.LoadRecordById<PersonModel>("Users", new Guid("240569c5-85ec-4fd7-99a5-5723ddfccea8"));
            // 3
            // oneRec.DateOfBirth = new DateTime(1985, 3, 25, 0, 0, 0, DateTimeKind.Utc);
            // db.UpsertRecord<PersonModel>("Users", oneRec.Id, oneRec);

            //5
            db.DeleteRecord<PersonModel>("Users", oneRec.Id);

            Console.ReadLine();
        }
    }

    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AddressModel PrimaryAddress { get; set; }

        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }

    }

    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string city { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }


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


        public List<T> LoadRecords<T>(string table)
        {

            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
            //new BsonDocument() => like select *
            // and it a better way, no filtering/processing... depends on size (to big of a dataset it's not practicle)
        }

        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id); // it's not js, we need dynamic capabilities on unknown T data type 
            return collection.Find(filter).First();
        }


        /**
        UpsertRecord:
        - create a binData to search by
        - look for a record with this data
        - found it? great now replace
        - didn't find it? ok, then add a nerw one i.e IsUpsert = true! 
        */
        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            BsonBinaryData binData = new BsonBinaryData(id, GuidRepresentation.Standard);
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
               new BsonDocument("_id", binData),
                record,
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);

        }

    }
}
