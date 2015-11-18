using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class Conversation
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("participants")]
        public List<ObjectId> Participants { get; set; }

        [BsonElement("messages")]
        public List<Message> Messages { get; set; }
    }
}
