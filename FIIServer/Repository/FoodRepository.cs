using System.Collections.Generic;
using System.Threading.Tasks;
using FIIServer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FIIServer.Repository
{
    public class FoodRepository
    {
        public static MongoClientSettings _settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@intolerances.dmppl.mongodb.net/Items?retryWrites=true&w=majority");
        public static MongoClient _client = new MongoClient(_settings);
        public static IMongoDatabase _database = _client.GetDatabase("Items");
        public IMongoCollection<Food> _collection = _database.GetCollection<Food>("Food");
        public async Task<IEnumerable<Food>> GetAll()
        {
            var listFoods = _collection.Find(g => true).ToList();
            return listFoods;
        }
    }


}
