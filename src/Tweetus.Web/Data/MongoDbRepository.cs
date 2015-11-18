using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public MongoDbRepository()
        {
            this._client = new MongoClient();
            this._database = _client.GetDatabase("tweetusdb");

            this._tweetCollection = _database.GetCollection<Tweet>("tweets");
            this._userFollowsCollection = _database.GetCollection<UserFollows>("user_follows");
            this._userLikesCollection = _database.GetCollection<UserLikes>("user_likes");
            this._conversationCollection = _database.GetCollection<Conversation>("conversations");
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
    }
}
