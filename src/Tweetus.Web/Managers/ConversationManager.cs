using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Tweetus.Web.Data;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.Utilities.Extensions;
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
            var conversationObjectId = conversationId.ToObjectId();

            var conversation = await _repository.Conversations.Find(c => c.Id == conversationObjectId).FirstOrDefaultAsync();

            return conversation;
        }

        public async Task<List<Conversation>> GetUserConversations(string userId)
        {
            var userObjectId = userId.ToObjectId();

            var conversations = await _repository.Conversations.Find(c => c.Participants.Contains(userObjectId)).ToListAsync();

            return conversations;
        }

        public async Task<Conversation> StartNewConversation(ObjectId userId, List<Tuple<ObjectId, string>> participants, string message)
        {
            var participantObjetcIds = participants.Select(p => p.Item1).ToList();
            var participantUsernames = participants.Select(p => p.Item2).ToList();

            var conversation = new Conversation()
            {
                Name = string.Join(", ", participantUsernames),
                Participants = participantObjetcIds,
                Messages = new List<Message>() {
                    new Message()
                    {
                        Id = ObjectId.GenerateNewId(),
                        Content = message,
                        UserIdFrom = userId,
                        SentOn = DateTime.Now
                    }
                }   
            };

            await _repository.Conversations.InsertOneAsync(conversation);

            return conversation;
        }

        public async Task<Message> SendNewMessage(string conversationId, string userId, string message)
        {
            var userObjectId = userId.ToObjectId();
            var conversationObjectId = conversationId.ToObjectId();

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
