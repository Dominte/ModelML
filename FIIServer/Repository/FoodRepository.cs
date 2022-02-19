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
        public static IMongoDatabase Database;
        public IMongoCollection<Food> Collection;
         
        public FoodRepository(IMongoClient client)
        {
            Database = Client.GetDatabase("Items");
            Collection = Database.GetCollection<Food>("Food");
        }

        public async Task<IEnumerable<Food>> GetAll()
        {
            var listFoods = Collection.Find(g => true).ToList();
            return listFoods;
        }

        public async Task<Food> GetByName(string name)
        {
            var food = Collection.Find(g => g.Name == name).ToList();
            return food[0];
        }

        public async Task<Food> GetById(int id)
        {
            var food = Collection.Find(g => g.Id == id).ToList();

            if (food.Count == 0)
            {
                return new Food(0);
            }

            return food[0];
        }

        public async Task AddFood(Food food)
        {
            await Collection.InsertOneAsync(food);
        }
    }
}
