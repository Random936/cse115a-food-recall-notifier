using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Food_Recall_Notif.Model
{
    public class Food_Item
    {
        [BsonElement("upc")] // Maps to the "upc" field in the MongoDB document
        public required string upc { get; set; }

        [BsonElement("date")] // Maps to the "date" field in the MongoDB document
        public DateTime date { get; set; }

        [BsonElement("brand")] // Maps to the "brand" field in the MongoDB document
        public required string brand { get; set; }

        [BsonElement("description")] // Maps to the "description" field in the MongoDB document
        public required string description { get; set; }
    }
    public class UPC_Item
    {
        [BsonElement("upc")] // Maps to the "upc" field in the MongoDB document
        public required string upc { get; set; }

        [BsonElement("date")] // Maps to the "date" field in the MongoDB document
        public DateTime date { get; set; }

        [BsonElement("date_scraped")] // Maps to the "brand" field in the MongoDB document
        public required string date_scraped { get; set; }

        [BsonElement("brand")] // Maps to the "description" field in the MongoDB document
        public required string brand { get; set; }
        [BsonElement("page")]
        public required string page { get; set; }
        [BsonElement("description")]
        public required string description { get; set; }
        [BsonElement("product_type")]
        public required string product_type { get; set; }
        [BsonElement("recall_reason")]
        public required string recall_reason { get; set; }
        [BsonElement("company")]
        public required string company { get; set; }
        [BsonElement("terminated")]
        public required bool terminated { get; set; }


    }
}
