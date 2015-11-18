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
    public class UserRelationshipManager : BaseManager
    {
        public UserRelationshipManager(IMongoDbRepository repository) : base(repository)
        {
        }

        public async Task AddFollowingUser(ObjectId userId, ObjectId followingId)
        {
            var existingUserFollowing = await _repository.UserFollows.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (existingUserFollowing == null)
            {
                await _repository.UserFollows.InsertOneAsync(new UserFollows()
                {
                    UserId = userId,
                    FollowingUserIds = new List<ObjectId>() { followingId }
                });
            }
            else
            {
                existingUserFollowing.FollowingUserIds.Add(followingId);
                await _repository.UserFollows.ReplaceOneAsync(a => a.UserId == userId, existingUserFollowing);
            }
        }

        public async Task<List<ObjectId>> GetFollowingUsers(ObjectId userId)
        {
            var userFollowing = await _repository.UserFollows.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (userFollowing == null)
                return new List<ObjectId>();
            else
                return userFollowing.FollowingUserIds;
        }

        public async Task<bool> IsUserFollowing(ObjectId userId, ObjectId followingId)
        {
            var userIsFollowing = await _repository.UserFollows.Find(u => u.UserId == userId && u.FollowingUserIds.Contains(followingId)).FirstOrDefaultAsync();

            return userIsFollowing != null;
        }

        public async Task<long> GetUserFollowingCount(ObjectId userId)
        {
            var userFollowing = await _repository.UserFollows.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (userFollowing == null)
                return 0;
            else
                return userFollowing.FollowingUserIds.Count;
        }

        public async Task<long> GetUserFollowerCount(ObjectId userId)
        {
            return await _repository.UserFollows.Find(u => u.FollowingUserIds.Contains(userId)).CountAsync();
        }
    }
}
