using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [BsonElement("file_content")]
        public byte[] FileContent { get; set; }

        [BsonElement("mime_type")]
        public string FileMimeType { get; set; }

        [BsonElement("filename")]
        public string FileOriginalName { get; set; }

        [BsonElement("created_on")]
        public DateTime CreatedOn { get; set; }
    }
}
