using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Food_Recall_Notif.Model
{
    public class Food_Item
    {
        [BsonId] // This is the unique identifier for MongoDB documents
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("date")] // Maps to the "date" field in the MongoDB document
        public DateTime Date { get; set; }

        [BsonElement("brand")] // Maps to the "brand" field in the MongoDB document
        public required string Brand { get; set; }

        [BsonElement("page")] // Maps to the "page" field in the MongoDB document
        public required string Page { get; set; }

        [BsonElement("description")] // Maps to the "description" field in the MongoDB document
        public required string Description { get; set; }

        [BsonElement("product_type")] // Maps to the "product_type" field in the MongoDB document
        public required string Product_type { get; set; }

        [BsonElement("recall_reason")] // Maps to the "recall_reason" field in the MongoDB document
        public required string Recall_reason { get; set; }

        [BsonElement("company")] // Maps to the "company" field in the MongoDB document
        public required string Company { get; set; }

        [BsonElement("terminated")] // Maps to the "terminated" field in the MongoDB document
        public bool Terminated { get; set; }
    }
}
