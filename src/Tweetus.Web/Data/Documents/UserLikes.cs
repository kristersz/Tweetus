using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class UserLikes
    {
        /// <summary>
        /// The user id
        /// </summary>
        [BsonId]
        public ObjectId UserId { get; set; }

        /// <summary>
        /// List of tweets that this user likes
        /// </summary>
        [BsonElement("tweet_ids")]
        public List<ObjectId> LikedTweetIds { get; set; }
    }
}
