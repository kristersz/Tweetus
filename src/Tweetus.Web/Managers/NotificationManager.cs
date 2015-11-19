using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Tweetus.Web.Data;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Data.Enums;

namespace Tweetus.Web.Managers
{
    public class NotificationManager : BaseManager
    {
        public NotificationManager(IMongoDbRepository repository) : base(repository)
        {
        }

        public async Task CreateNewNotification(ObjectId notifiedUserId, NotificationType type, ObjectId userId, ObjectId? tweetId)
        {
            var existingUserNotification = await _repository.UserNotifications.Find(n => n.UserId == notifiedUserId).FirstOrDefaultAsync();

            var newNotification = new Notification();

            newNotification.Type = (int)type;
            newNotification.UserId = userId;
            newNotification.CreatedOn = DateTime.Now;

            if (tweetId.HasValue)
                newNotification.TweetId = tweetId.Value;

            if (existingUserNotification == null)
            {
                var userNotification = new UserNotifications();

                userNotification.UserId = notifiedUserId;
                userNotification.Notifications = new List<Notification>() { newNotification };

                await _repository.UserNotifications.InsertOneAsync(userNotification);
            }
            else
            {
                existingUserNotification.Notifications.Add(newNotification);
                await _repository.UserNotifications.ReplaceOneAsync(n => n.Id == existingUserNotification.Id, existingUserNotification);
            }
        }

        public async Task<UserNotifications> GetUserNotifications(ObjectId userId)
        {
            var existingUserNotification = await _repository.UserNotifications.Find(n => n.UserId == userId).FirstOrDefaultAsync();

            if (existingUserNotification == null)
            {
                return null;
            }
            else
            {
                return existingUserNotification;
            }
        }
    }
}
