using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using MongoDB.Bson;
using Tweetus.Web.Managers;
using Tweetus.Web.Models;
using Tweetus.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Tweetus.Web.Controllers
{
    [Authorize]
    public class ConversationController : BaseController
    {
        private readonly ConversationManager _conversationManager;

        public ConversationController(UserManager<ApplicationUser> userManager, ConversationManager conversationManager) : base(userManager)
        {
            _conversationManager = conversationManager;
        }

        public async Task<IActionResult> List()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            var conversations = await _conversationManager.GetUserConversations(user.Id);

            var viewModels = new List<ConversationVM>();

            foreach (var conversation in conversations)
            {
                var conversationVM = new ConversationVM();

                var participatingUsers = new List<string>();

                foreach (var participant in conversation.Participants)
                {
                    if (participant.ToString() == user.Id)
                    {
                        participatingUsers.Add("You");
                    }
                    else
                    {
                        var partUser = await _userManager.FindByIdAsync(participant.ToString());
                        participatingUsers.Add(partUser.UserName);
                    }
                }

                conversationVM.ConversationId = conversation.Id.ToString();
                conversationVM.Name = string.Join(", ", participatingUsers);

                var lastMessage = conversation.Messages.OrderByDescending(m => m.SentOn).FirstOrDefault();

                var messageVM = new MessageVM();
                var messageUser = await _userManager.FindByIdAsync(lastMessage.UserIdFrom.ToString());

                messageVM.MessageId = lastMessage.Id.ToString();
                messageVM.Username = messageUser.UserName;
                messageVM.Content = lastMessage.Content;
                messageVM.SentOn = lastMessage.SentOn;

                conversationVM.Messages.Add(messageVM);

                viewModels.Add(conversationVM);
            }

            return View(viewModels);
        }

        [HttpPost]
        public async Task<JsonResult> StartNewConversation(string username, string message)
        {
            var result = new JsonServiceResult<bool>();

            try
            {
                var user = await _userManager.FindByIdAsync(User.GetUserId());
                var recipientUser = await _userManager.FindByNameAsync(username.ToUpper());

                if (recipientUser == null)
                {
                    result.Message = "User with the specified username was not found!";
                    return Json(result);
                }

                await _conversationManager.StartNewConversation(user.Id, recipientUser.Id, message);

                result.Value = result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> OpenConversation(string conversationId)
        {
            var result = new JsonServiceResult<ConversationVM>();

            try
            {
                var conversation = await _conversationManager.GetConversationById(conversationId);
                var conversationVM = new ConversationVM();

                conversationVM.ConversationId = conversation.Id.ToString();
                conversationVM.Name = "ConversationName";

                foreach (var message in conversation.Messages)
                {
                    var messageVM = new MessageVM();
                    var messageUser = await _userManager.FindByIdAsync(message.UserIdFrom.ToString());

                    messageVM.MessageId = message.Id.ToString();
                    messageVM.Username = messageUser.UserName;
                    messageVM.Content = message.Content;
                    messageVM.SentOn = message.SentOn;

                    conversationVM.Messages.Add(messageVM);
                }

                result.Value = conversationVM;
                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> SendNewMessage(string conversationId, string message)
        {
            var result = new JsonServiceResult<MessageVM>();

            try
            {
                var newMessage = await _conversationManager.SendNewMessage(conversationId, User.GetUserId(), message);

                result.Value = new MessageVM()
                {
                    MessageId = newMessage.Id.ToString(),
                    Username = User.GetUserName(),
                    Content = newMessage.Content,
                    SentOn = newMessage.SentOn
                };

                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result);
        }
    }
}
