using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Tweetus.Web.Data.Documents;

namespace Tweetus.Web.Data
{
    public class MongoDbRepository : IMongoDbRepository
    {
        protected readonly IMongoClient _client;
        protected readonly IMongoDatabase _database;

        protected readonly IMongoCollection<Tweet> _tweetCollection;
        protected readonly IMongoCollection<UserFollows> _userFollowsCollection;
        protected readonly IMongoCollection<UserLikes> _userLikesCollection;
        protected readonly IMongoCollection<Conversation> _conversationCollection;
        protected readonly IMongoCollection<UserNotifications> _userNotificationCollection;

        public MongoDbRepository(IOptions<AppSettings> appSettings)
        {
            _client = new MongoClient(appSettings.Value.MongoDbConnectionString);
            _database = _client.GetDatabase(appSettings.Value.MongoDbName);

            _tweetCollection = _database.GetCollection<Tweet>("tweets");
            _userFollowsCollection = _database.GetCollection<UserFollows>("user_follows");
            _userLikesCollection = _database.GetCollection<UserLikes>("user_likes");
            _conversationCollection = _database.GetCollection<Conversation>("conversations");
            _userNotificationCollection = _database.GetCollection<UserNotifications>("user_notifications");
        }

        public IMongoDatabase Database
        {
            get
            {
                return _database;
            }
        }

        public IMongoCollection<Tweet> Tweets
        {
            get
            {
                return _tweetCollection;
            }
        }

        public IMongoCollection<UserFollows> UserFollows
        {
            get
            {
                return _userFollowsCollection;
            }
        }

        public IMongoCollection<UserLikes> UserLikes
        {
            get
            {
                return _userLikesCollection;
            }
        }

        public IMongoCollection<Conversation> Conversations
        {
            get
            {
                return _conversationCollection;
            }
        }

        public IMongoCollection<UserNotifications> UserNotifications
        {
            get
            {
                return _userNotificationCollection;
            }
        }
    }
}
