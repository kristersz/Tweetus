using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class Notification
    {
        [BsonElement("type")]
        public int Type { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonElement("tweet_id")]
        public ObjectId TweetId { get; set; }

        [BsonElement("created_on")]
        public DateTime CreatedOn { get; set; }
    }
}
