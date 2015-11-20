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
using Tweetus.Web.Services.Mappers;
using Tweetus.Web.Utilities.Extensions;
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

                conversationVM.ConversationId = conversation.Id.ToString();
                conversationVM.Name = conversation.Name;

                var lastMessage = conversation.Messages.OrderByDescending(m => m.SentOn).FirstOrDefault();
                var messageUser = await _userManager.FindByIdAsync(lastMessage.UserIdFrom.ToString());

                conversationVM.Messages.Add(MessageMapper.MapMessageToViewModel(lastMessage, messageUser.UserName));

                viewModels.Add(conversationVM);
            }

            return View(viewModels);
        }

        [HttpPost]
        public async Task<JsonResult> StartNewConversation(string username, string message)
        {
            var result = new JsonServiceResult<ConversationVM>();

            try
            {
                var user = await _userManager.FindByIdAsync(User.GetUserId());
                var recipientUser = await _userManager.FindByNameAsync(username.ToUpper());

                if (recipientUser == null)
                {
                    result.Message = "User with the specified username was not found!";
                    return Json(result);
                }

                List<Tuple<ObjectId, string>> participants = new List<Tuple<ObjectId, string>>();

                participants.Add(new Tuple<ObjectId, string>(user.Id.ToObjectId(), user.UserName));
                participants.Add(new Tuple<ObjectId, string>(recipientUser.Id.ToObjectId(), recipientUser.UserName));

                var conversation = await _conversationManager.StartNewConversation(user.Id.ToObjectId(), participants, message);

                result.Value = new ConversationVM()
                {
                    ConversationId = conversation.Id.ToString(),
                    Name = conversation.Name,
                    Messages = new List<MessageVM>() { MessageMapper.MapMessageToViewModel(conversation.Messages[0], User.GetUserName()) }
                };

                result.IsValid = true;
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
                conversationVM.Name = conversation.Name;

                foreach (var message in conversation.Messages)
                {
                    var messageUser = await _userManager.FindByIdAsync(message.UserIdFrom.ToString());
                    conversationVM.Messages.Add(MessageMapper.MapMessageToViewModel(message, messageUser.UserName));
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

                result.Value = MessageMapper.MapMessageToViewModel(newMessage, User.GetUserName());
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
