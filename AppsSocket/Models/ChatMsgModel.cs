using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppsSocket.Models
{
    public class ChatMsgModel
    {
        public string fromId { get; set; }
        public string toId { get; set; }
        public string strMsg { get; set; }
        public string iconClass { get; set; }
        public DateTime? createDte { get; set; }
    }
}