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
    public class Testhub :Hub
    {
        static List<TestModel> UsersList = new List<TestModel>();
        public void Connect(string name)
        {
            try
            {
                var id = Context.ConnectionId;
                UsersList.Add(new TestModel { connectionId = id, username = name });//add caller to user chat

                Clients.Caller.getId("you are now connected: conection is " + id);//get my friends online
            }
            catch (Exception ex)
            {

            }
        }

        public void Sentmsg(string mesage,string username)
        {
            MsgModel msg = new MsgModel();
            msg.Message = mesage;
            msg.UserName = username;
            Clients.All.getMsg(msg);
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