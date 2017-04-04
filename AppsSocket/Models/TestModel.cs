using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppsSocket.Models
{
    public class TestModel
    {
        public string connectionId { get; set; }
        public string username { get; set; }
    }

    public class MsgModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}