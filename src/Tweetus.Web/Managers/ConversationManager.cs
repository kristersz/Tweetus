using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Tweetus.Web.Data;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.ViewModels;

namespace Tweetus.Web.Managers
{
    public class ConversationManager : BaseManager
    {
        public ConversationManager(IMongoDbRepository repository) : base(repository)
        {
        }

        public async Task<Conversation> GetConversationById(string conversationId)
        {
            var conversationObjectId = new ObjectId(conversationId);

            var conversation = await _repository.Conversations.Find(c => c.Id == conversationObjectId).FirstOrDefaultAsync();

            return conversation;
        }

        public async Task<List<Conversation>> GetUserConversations(string userId)
        {
            var userObjectId = new ObjectId(userId);

            var conversations = await _repository.Conversations.Find(c => c.Participants.Contains(userObjectId)).ToListAsync();

            return conversations;
        }

        public async Task StartNewConversation(string userIdFrom, string userIdTo, string message)
        {
            var userFromObjectId = new ObjectId(userIdFrom);
            var userToobjectId = new ObjectId(userIdTo);

            var conversation = new Conversation()
            {
                Participants = new List<ObjectId>() { userFromObjectId, userToobjectId },
                Messages = new List<Message>() {
                    new Message()
                    {
                        Id = ObjectId.GenerateNewId(),
                        Content = message,
                        UserIdFrom = userFromObjectId,
                        SentOn = DateTime.Now
                    }
                }   
            };

            await _repository.Conversations.InsertOneAsync(conversation);
        }

        public async Task<Message> SendNewMessage(string conversationId, string userId, string message)
        {
            var userObjectId = new ObjectId(userId);
            var conversationObjectId = new ObjectId(conversationId);

            var existingConversation = await GetConversationById(conversationId);

            var newMessage = new Message()
            {
                Id = ObjectId.GenerateNewId(),
                Content = message,
                UserIdFrom = userObjectId,
                SentOn = DateTime.Now
            };

            existingConversation.Messages.Add(newMessage);

            await _repository.Conversations.ReplaceOneAsync(c => c.Id == conversationObjectId, existingConversation);

            return newMessage;
        }
    }
}
