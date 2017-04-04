using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppsSocket.Models
{
    public class UserHubModel
    {
        public string connectionId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public int freeFlag { get; set; }
        //if freeflag==0 ==> Busy
        //if freeflag==1 ==> Free
       

        //if tpflag==2 ==> User Admin
        //if tpflag==0 ==> User Member
        //if tpflag==1 ==> Admin

       // public string tpflag { get; set; }

       // public int UserID { get; set; }
        //public int AdminID { get; set; } 
    }
}