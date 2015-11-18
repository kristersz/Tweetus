using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class UserFollows
    {
        /// <summary>
        /// The user id
        /// </summary>
        [BsonId]
        public ObjectId UserId { get; set; }

        /// <summary>
        /// List of users that this user is following
        /// </summary>
        [BsonElement("following_ids")]
        public List<ObjectId> FollowingUserIds { get; set; }
    }
}
