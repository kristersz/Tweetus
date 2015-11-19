using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Tweetus.Web.Data.Documents;

namespace Tweetus.Web.Data
{
    public interface IMongoDbRepository
    {
        IMongoDatabase Database { get; }
        IMongoCollection<Tweet> Tweets { get; }
        IMongoCollection<UserFollows> UserFollows { get; }
        IMongoCollection<UserLikes> UserLikes { get; }
        IMongoCollection<Conversation> Conversations { get; }
        IMongoCollection<UserNotifications> UserNotifications { get; }
    }
}
