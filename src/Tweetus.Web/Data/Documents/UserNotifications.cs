using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tweetus.Web.Data.Documents
{
    public class UserNotifications
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonElement("notifications")]
        public List<Notification> Notifications { get; set; }
    }
}
