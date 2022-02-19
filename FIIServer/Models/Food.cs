using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FIIServer.Models
{
    public class Food
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("barcode")]
        public string Barcode { get; set; }

        [BsonElement("lactoseFlag")]
        public bool LactoseFlag { get; set; }

        [BsonElement("glutenFlag")]
        public bool GlutenFlag { get; set; }

        [BsonElement("caffeineFlag")]
        public bool CaffeineFlag { get; set; }

        [BsonElement("eggFlag")]
        public bool EggFlag { get; set; }

        [BsonElement("fishFlag")]
        public bool FishFlag { get; set; }

        [BsonElement("peanutsFlag")]
        public bool PeanutsFlag { get; set; }

        [BsonElement("almondsFlag")]
        public bool AlmondsFlag { get; set; }

        public Food(int id)
        {
            Id = id;
        }
    }
}
