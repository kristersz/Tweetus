using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetus.Web.Data.Documents;
using Tweetus.Web.ViewModels;

namespace Tweetus.Web.Services.Mappers
{
    public class MessageMapper
    {
        public static MessageVM MapMessageToViewModel(Message message, string userName)
        {
            return new MessageVM()
            {
                MessageId = message.Id.ToString(),
                Username = userName,
                Content = message.Content,
                SentOn = message.SentOn
            };
        }
    }
}
