using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using AppsSocket.Models;
using AppsSocket.SQLData;
using System.IO;

namespace AppsSocket
{
    public class ChatHub:Hub
    {
        static List<UserHubModel> UsersList = new List<UserHubModel>();
        static List<ChatMsgModel> chatmsg = new List<ChatMsgModel>();

        public void Connect(string employeeId,string userName,string sessionId)
        {
            try
            {
                var id = Context.ConnectionId;

                if (UsersList.Where(wr => wr.userId == employeeId && wr.sessionId == sessionId).Count() > 0)
                {
                    UsersList.Remove(UsersList.Where(wr => wr.userId == employeeId && wr.sessionId == sessionId).FirstOrDefault());//remove Previous caller account
                }


                UsersList.Add(new UserHubModel { connectionId = id, userId = employeeId, userName = userName, freeFlag = 1, sessionId = sessionId });//add caller to user chat

                //my friend online
                var frindOnlineExceptMe = UsersList.Except(UsersList.Where(wr => wr.userId == employeeId).ToList()).Select(aa => new { aa.userId, aa.userName, msgCount = countUnreadMsg(aa.userId, employeeId) }).Distinct().ToList();

                //notify my friend in new online person
                foreach (var row in UsersList)
                {
                    if (row.userId != employeeId)
                    {
                        var frindOnlineExceptMeWithConnectionID = UsersList.Except(UsersList.Where(wr => wr.userId == row.userId).ToList()).Select(aa => new { aa.userId, aa.userName, msgCount = countUnreadMsg(aa.userId,row.userId) }).Distinct().ToList();

                        Clients.Client(row.connectionId).getOnline(frindOnlineExceptMeWithConnectionID);//notify my friend except me
                    }

                }

                Clients.Caller.getOnline(frindOnlineExceptMe);//get my friends online
            }
            catch (Exception ex)
            {

            }
        }

        //view online friends
        public void ViewOnlineFriend(string employeeId)
        {
            var frindOnlineExceptMe = UsersList.Except(UsersList.Where(wr => wr.userId == employeeId).ToList()).Select(aa => new { aa.userId, aa.userName, msgCount = countUnreadMsg(aa.userId, employeeId) }).Distinct().ToList();

            foreach (var row in UsersList)
            {
                if (row.userId != employeeId)
                {
                    var frindOnlineExceptMeWithConnectionID = UsersList.Except(UsersList.Where(wr => wr.userId == row.userId).ToList()).Select(aa => new { aa.userId, aa.userName, msgCount = countUnreadMsg(aa.userId, row.userId) }).Distinct().ToList();

                    Clients.Client(row.connectionId).getOnline(frindOnlineExceptMeWithConnectionID);
                }

            }
            Clients.Caller.getOnline(frindOnlineExceptMe);
        }

        public int countUnreadMsg(string fromId, string toId)
        {
            using (ApplicantDBEntities db = new ApplicantDBEntities())
            {
                var qMsg = db.core_chat.Where(wr => wr.msgfrom == fromId && wr.msgto == toId && wr.isread==0).Count();
                return qMsg;
            }
        }

        public void Viewmsg(string fromId, string toId,string toName)
        {
            List<ChatMsgModel> chatmsg = new List<ChatMsgModel>();

            using (ApplicantDBEntities db = new ApplicantDBEntities())
            {
                var qchatFromMe = db.core_chat.Where(wr => wr.msgfrom == fromId && wr.msgto == toId).OrderByDescending(aa => aa.createddate).Take(10).ToList();
                var qchatToMe = db.core_chat.Where(wr => wr.msgfrom == toId && wr.msgto == fromId).OrderByDescending(aa => aa.createddate).Take(10).ToList();


                foreach (var row in qchatFromMe.OrderBy(aa => aa.createddate))
                {
                    chatmsg.Add(new ChatMsgModel { fromId = row.msgfrom, toId = row.msgto, strMsg = row.msg, iconClass = "bubble bubble--alt", createDte = row.createddate });
                }

                foreach (var row in qchatToMe.OrderBy(aa => aa.createddate))
                {
                    chatmsg.Add(new ChatMsgModel { fromId = row.msgfrom, toId = row.msgto, strMsg = "<img src='http://apps.fastgroup.biz/201pic/48px/" + toId + ".jpg' width='32' height='32' />&nbsp;<strong>" + toName + "</strong><br/>" + row.msg, iconClass = "bubble", createDte = row.createddate });
                }

                var msglist = chatmsg.OrderBy(aa => aa.createDte).ToList();

                //begin:update msg status to read
                var qMsg = db.core_chat.Where(wr => wr.msgfrom == toId && wr.msgto == fromId && wr.isread == 0);
                foreach (var row in qMsg)
                {
                    row.isread = 1;
                }
                db.SaveChanges();
                //end:

                //begin: broard cast info
                var frindOnlineExceptMe = UsersList.Except(UsersList.Where(wr => wr.userId == fromId).ToList()).Select(aa => new { aa.userId, aa.userName, msgCount = countUnreadMsg(aa.userId, fromId) }).Distinct().ToList();
                Clients.Caller.getOnline(frindOnlineExceptMe);
                Clients.Caller.getfriendmsg(msglist);
                //end:
            }
        }
        public void SendMsg(string fromId, string toId, string strMsg)
        {
            //chatmsg.Add(new ChatMsgModel { fromId = fromId, toId = toId, strMsg = strMsg, createDte = DateTime.Now });

            using (ApplicantDBEntities db = new ApplicantDBEntities())
            {
                core_chat chatinfo = new core_chat
                {
                    chatId = 0,
                    msgfrom = fromId,
                    msgto = toId,
                    msg = strMsg,
                    createdby = fromId,
                    createddate = DateTime.Now,
                    isread=0
                };

                db.core_chat.Add(chatinfo);
                db.SaveChanges();
            }

            foreach(var row in UsersList.Where(wr=> wr.userId==toId)){
                Clients.Client(row.connectionId).getChatMsg(fromId, toId,strMsg,DateTime.Now );             
            }

           
            
        }

        public override Task OnConnected()
        {

            Console.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            return (base.OnConnected());
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                var id = Context.ConnectionId;
                UsersList.Remove(UsersList.Where(wr => wr.connectionId == id).FirstOrDefault());//remove Previous caller account
            }
            else
            {
                // This server hasn't heard from the client in the last ~35 seconds.
                // If SignalR is behind a load balancer with scaleout configured, 
                // the client may still be connected to another SignalR server.
            }

            return (base.OnDisconnected(stopCalled));
        }


        public override Task OnReconnected()
        {
            Console.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
            return (base.OnDisconnected(false));
        }

    }
}