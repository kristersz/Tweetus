using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class ConversationVM
    {
        public ConversationVM()
        {
            Messages = new List<MessageVM>();
        }

        public string ConversationId { get; set; }
        public string Name { get; set; }
        public List<MessageVM> Messages { get; set; }
    }
}
