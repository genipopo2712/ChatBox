using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using ChatBox.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using System.Globalization;

namespace ChatBox
{
    public class ChatHub : Hub
    {
        private static List<string> connectedClients = new List<string>();

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("SendMessage", $"{Context.ConnectionId} has joined the group {groupName}.");
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("SendMessage", $"{Context.ConnectionId} has left the group {groupName}.");
        }
        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("SendMessage", $"{Context.ConnectionId}: {message}");
        }
        public override async Task OnConnectedAsync()
        {
            connectedClients.Add(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            connectedClients.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        IMemberRepository memberRepository;
        IMessageRepository messageRepository;
        IConversationRepository conversationRepository;
        public ChatHub(IMemberRepository memberRepository, IMessageRepository messageRepository, IConversationRepository conversationRepository)
        {
            this.memberRepository = memberRepository;
            this.messageRepository = messageRepository;
            this.conversationRepository = conversationRepository;
        }

        public void Send(string user, string message,string convid)
        {
            
            Message obj = new Message
            {
                UserId = user,
                Content = message,
                ConvId= convid
            };
            int ret = messageRepository.Add(obj);
            if (ret > 0)
            {
                Member member = memberRepository.GetMemberById(user);
                obj.MessageDate = DateTime.Now;
                obj.Fullname = member.Fullname;
                obj.UserId = user;
                obj.Avatar = member.Avatar;
            }
            //obj.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //messageRepository.Add(obj);
            //return Redirect($"/Chat/Chat?t={a}");
            Clients.Group(convid).SendAsync("recMsg",obj);
        }
        //Note 02: Create funtion notification for user is typing
        public async Task Usertyping(string userid, string convid, string input)
        {
            bool type = false;
            if (input != null)
            {
                if (input.Length >= 5)
                {
                    type = true;
                }
                else
                {
                    type = false;
                }
            }
            await Clients.OthersInGroup(convid).SendAsync("typeStatus", type);
        }
        //Note 03: create a structure for the object to send to the client
        public async Task Userstatus(string userid, string convid,string lastActive)
        {
            UserStatus obj = new UserStatus
            {
                uId = userid,
                uN = conversationRepository.GetMembersInGroup(userid, convid),
                cId = convid,
                nt = "Available",
                p= "success",
                d= "online"
            };            
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(lastActive, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(Indochina Time)'", CultureInfo.InvariantCulture);
            DateTime lastActivetime = dateTimeOffset.LocalDateTime;
            DateTime act = DateTime.Now;
            //string id = conversationRepository.GetMembersIdInGroup(userid, convid);
            //UserStatus user = memberRepository.GetTimeAwayById(id);
            var gap = act - lastActivetime;
            int a = 0;
            string b = "min";   
            //if (valInput != null)
            //{
            //    if(valInput != "Type message")
            //    {
            //        obj.type = true;
            //    }
            //    else
            //    {
            //        obj.type = false;
            //    }
            //}
            if (gap != null)
            {
                a = (int)gap.TotalMinutes;
                if (a > 1) { b = "mins"; }
                else if (a > 60)
                {
                    a = (int)gap.TotalHours;
                    if (a <= 1)
                    {
                        b = "hour";

                    }
                    else if((a>1) && (a<24))
                    {
                        b = "hours";

                    }
                    else if (a>=24)
                    {
                        a = (int)gap.TotalDays;
                        if (a <= 1)
                        {
                            b = "day";

                        }
                        else
                        {
                            b = "days";
                        }
                    }                          
                }
            }
            if (gap.TotalMinutes > 1)
            {

                obj.d = "away";
                obj.nt = $"Active {a} {b} ago";
                obj.p = "gray-600";
            }
            else
            {
                obj.d = "online";
                obj.nt = "Available";
                obj.p = "success";
            }
            

            await Clients.OthersInGroup(convid).SendAsync("setStatus",obj);

            //Clients.All.SendAsync("setStatus",status,time,color);
        }
        public async Task Readmessage(string convid, string userid)
        {
            string i = "";
            int ret = messageRepository.ReadMessage(convid, userid);
            if (ret > 0)
            {
                i = "text-success";
            }
            await Clients.Group(convid).SendAsync("messIsRead", i);
        }
    }
}
