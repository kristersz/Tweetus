using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class Tweet
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("mime_type")]
        public string FileMimeType { get; set; }

        [BsonElement("file_path")]
        public string FilePath { get; set; }

        [BsonElement("retweeted_from_id")]
        public ObjectId RetweetedFromId { get; set; }

        [BsonElement("retweeted_from_username")]
        public string RetweetedFromUserName { get; set; }

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        [BsonElement("created_on")]
        public DateTime CreatedOn { get; set; }
    }
}
