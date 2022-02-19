using FIIServer.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIIServer.Repository
{
    public class FoodRepository
    {
        public static MongoClientSettings _settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@intolerances.dmppl.mongodb.net/Items?retryWrites=true&w=majority");
        public static MongoClient Client = new MongoClient(_settings);
        public static IMongoDatabase Database = Client.GetDatabase("Items");
        public IMongoCollection<Food> Collection = Database.GetCollection<Food>("Food");
        public async Task<IEnumerable<Food>> GetAll()
        {
            var listFoods = Collection.Find(g => true).ToList();
            return listFoods;
        }
    }
}
