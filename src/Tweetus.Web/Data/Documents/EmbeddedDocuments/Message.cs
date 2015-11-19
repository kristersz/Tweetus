using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id_from")]
        public ObjectId UserIdFrom { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("sent_on")]
        public DateTime SentOn { get; set; }
    }
}
