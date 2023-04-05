using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using ChatBox.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace ChatBox
{
    public class ChatHub : Hub
    {
        private static List<string> connectedClients = new List<string>();

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
            Clients.All.SendAsync("recMsg",obj);
        }
        public async Task Userstatus(string userid, string convid)
        {   
            
            string pclass = "success";
            string time = "Available";
            string divclass = "online";
            string id = conversationRepository.GetMembersIdInGroup(userid, convid);
            UserStatus user = memberRepository.GetTimeAwayById(id);
            if (user != null)
            {
                if (user.aTimer >= 1)
                {
                    divclass = "away";
                    time = $"Active {user.Ago} {user.TimeName} ago";
                    pclass = "gray-600";
                }
                else
                {
                    divclass = "online";
                    time = "Available";
                    pclass = "success";
                }
            }
            string currentClient = Context.ConnectionId;
            foreach (string clientId in connectedClients)
            {
                if (clientId == currentClient)
                {
                    await Clients.Client(clientId).SendAsync("setStatus",time, divclass, pclass);
                }
            }
            //Clients.All.SendAsync("setStatus",status,time,color);
        }
    }
}
