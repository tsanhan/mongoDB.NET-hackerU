using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.NET
{
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
