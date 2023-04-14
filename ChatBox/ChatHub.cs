using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using ChatBox.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace ChatBox
{
    public class ChatHub : Hub
    {
        private static List<string> connectedClients = new List<string>();
        private readonly UserManager<IdentityUser> userManager;

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
            string name = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            connectedClients.Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, name);
            await Clients.All.SendAsync("SendMessage", $"{Context.ConnectionId} has joined the group {name}, by async.");
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
       
        public void Send(string senderId, string message,string convid)
        {            
            Message obj = new Message
            {
                UserId = senderId,
                Content = message,
                ConvId= convid
            };
            int ret = messageRepository.Add(obj);
            Member member = memberRepository.GetMemberById(senderId);
            if (ret > 0)
            {                
                obj.MessageDate = DateTime.Now;
                obj.Fullname = member.Fullname;
                obj.UserId = senderId;
                obj.Avatar = member.Avatar;
            }
            ConversationInfo con = new ConversationInfo
            {                
                Content = message,
                ConvId = convid,
                MessageDate = DateTime.ParseExact(DateTime.Now.ToString("hh:mm tt"), "hh:mm tt", CultureInfo.InvariantCulture),                
            };
            Clients.Group(convid).SendAsync("recMsg",obj,con, senderId);
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
        public async Task trackUserStatus(string userid, string convid,string lastActive)
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
            var gap = act - lastActivetime;
            int a = 0;
            string b = "min";   
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
        //public async Task Creategroup(string id, string userid)
        //{

        //    int ret = conversationRepository.Add(id);
        //}
    }
}
