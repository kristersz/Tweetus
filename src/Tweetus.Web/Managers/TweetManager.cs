using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Tweetus.Web.Data;
using Tweetus.Web.Data.Documents;

namespace Tweetus.Web.Managers
{
    public class TweetManager : BaseManager
    {
        public TweetManager(IMongoDbRepository repository) : base(repository)
        {
        }

        public async Task<Tweet> GetTweetById(ObjectId id)
        {
            return await _repository.Tweets.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Tweet>> GetTweetsByIds(List<ObjectId> ids)
        {
            return await _repository.Tweets.Find(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IList<Tweet>> GetTweetsByUserId(ObjectId userId)
        {
            var result = await _repository.Tweets
                .Find(t => t.UserId == userId)
                //.SortByDescending(t => t.CreatedOn)
                //.Limit(10)
                .ToListAsync();

            return result.ToList();
        }

        public async Task<IList<Tweet>> GetTweetsByUserIds(List<ObjectId> userIds)
        {
            var result = await _repository.Tweets.Find(t => userIds.Contains(t.UserId)).ToListAsync();
            return result.ToList();
        }

        public async Task<long> GetUserTweetCount(ObjectId userId)
        {
            return await _repository.Tweets.Find(t => t.UserId == userId).CountAsync();
        }

        public async Task CreateTweet(Tweet tweet)
        {
            await _repository.Tweets.InsertOneAsync(tweet);
        }

        public async Task DeleteTweet(ObjectId id)
        {
            await _repository.Tweets.DeleteOneAsync(t => t.Id == id);
        }

        public async Task LikeTweet(ObjectId userId, ObjectId tweetId)
        {
            var existingUserLikes = await _repository.UserLikes.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (existingUserLikes == null)
            {
                await _repository.UserLikes.InsertOneAsync(new UserLikes()
                {
                    UserId = userId,
                    LikedTweetIds = new List<ObjectId>() { tweetId }
                });
            }
            else
            {
                existingUserLikes.LikedTweetIds.Add(tweetId);
                await _repository.UserLikes.ReplaceOneAsync(a => a.UserId == userId, existingUserLikes);
            }
        }

        public async Task<IList<Tweet>> GetUserLikedTweets(ObjectId userId)
        {
            var userLikes = await _repository.UserLikes.Find(t => t.UserId == userId).FirstOrDefaultAsync();

            if (userLikes == null)
            {
                return new List<Tweet>();
            }
            else
            {
                return await GetTweetsByIds(userLikes.LikedTweetIds);
            }
        }
    }
}
