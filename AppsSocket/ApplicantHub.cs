using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using AppsSocket.Models;
using AppsSocket.SQLData;

namespace AppsSocket
{
    public class ApplicantHub : Hub
    {

    
        static List<UserHubModel> UsersList = new List<UserHubModel>();

        public void Connect(string userID)
        {
            var id = Context.ConnectionId;
            UsersList.Add(new UserHubModel { connectionId = id, userId = userID, freeFlag = 1 });

            Clients.Caller.onConnected(id, userID);
        }


        public void setApplicantLogin(string fname, string lname, DateTime bdate)
        {

            ApplicantData n = new ApplicantData();
            PromptModel promptModel =  n.checkBooking(fname,lname,bdate);

            Clients.All.getApplicantLogin(promptModel.functionMsg,promptModel.ifunctionVal);
           
            if (promptModel.ifunctionVal == 0)
            {
                Clients.All.getApplicantList(n.getApplicantLoginbyCode(promptModel.applicantID));
            }

        }

        public void getOnline(string exceptUserid)
        {
            List<UserHubModel> getUserExcept=UsersList.Where(aa=>aa.userId==exceptUserid).ToList();

            var qUserlist = UsersList.Except(getUserExcept);
            Clients.All.onTheLine(qUserlist);
        
        }


        public void GetMessageOfDay()
        {
            DateTime? nowdt = DateTime.Now.Date;
            //List<V_MSG_DAY> msgDay = dbcontext.V_MSG_DAYs.Where(aa => aa.DateExpired >= nowdt).ToList();
            Clients.All.getMessageOfDay("");
        }
        public void AddMessage(string name, string message)
        {
            Console.WriteLine("Hub AddMessage {0} {1}\n", name, message);
            Clients.All.addMessage(name, message);
        }

        public void Heartbeat()
        {
            Console.WriteLine("Hub Heartbeat\n");
            Clients.All.heartbeat();
        }

        public void SendHelloObject(HelloModel hello)
        {
            Console.WriteLine("Hub hello {0} {1}\n", hello.Molly, hello.Age);
            Clients.All.sendHelloObject(hello);
        }

        public override Task OnConnected()
        {

            Console.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            return (base.OnConnected());
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("Hub OnDisconnected {0}\n", Context.ConnectionId);
            return (base.OnDisconnected(stopCalled));
        }


        public override Task OnReconnected()
        {
            Console.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
            return (base.OnDisconnected(false));
        }
    }
}