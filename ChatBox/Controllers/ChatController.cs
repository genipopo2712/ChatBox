using Chatbox;
using ChatBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace ChatBox.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {

        IMemberRepository memberRepository;
        IMessageRepository messageRepository;
        IConversationRepository conversationRepository;
        private readonly IHubContext<ChatHub> chatHubContext;

        public ChatController(IMemberRepository memberRepository, IMessageRepository messageRepository, IConversationRepository conversationRepository, IHubContext<ChatHub> chatHubContext)
        {
            this.memberRepository = memberRepository;
            this.messageRepository = messageRepository;
            this.conversationRepository = conversationRepository;
            this.chatHubContext = chatHubContext;
        }
        [ServiceFilter(typeof(ContactFilter))]
        public IActionResult Index(string id)
        {
            return View();
        }
        [ServiceFilter(typeof(ContactFilter))]
        public IActionResult Chat(string t="")
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = conversationRepository.GetNameById(t);
            string ids = conversationRepository.GetMembersIdInGroup(id, t);
            DateTime lT = memberRepository.GetLastTimeActive(ids);
            ViewBag.lastActive = (int)(DateTime.Now - lT).TotalMinutes;
            if(string.IsNullOrEmpty(name))
            {
                name = conversationRepository.GetMembersInGroup(id, t).ToString();
                //Note 01: Not use this function any more from 06/04/23 because this make heavy traffic of query to database
                /*
                memberRepository.SetTimeActive(name, DateTime.Now);
                */
            }
            ViewBag.id = id;
            ViewBag.conv = t;
            ViewBag.chatname = name;
            
            ViewBag.messages = messageRepository.GetMessages(t);
            return View();
        }
        [HttpPost]
        public IActionResult AddMessage(Message obj,string a="")
        {
            obj.ConvId = a;
            obj.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            messageRepository.Add(obj);
            return Redirect($"/Chat/Chat?t={a}");
        }
        [ServiceFilter(typeof(ContactFilter))]
        public async Task<IActionResult> Create(string i="")
        {
            string us = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string a = Helper.StringConv(us, i);
            List<string> group = new List<string>();
            group.Add(i);
            group.Add(us);
            int ret = conversationRepository.Add(a, "", "");
            if (ret > 0)
            {
                foreach (var it in group)
                {
                    conversationRepository.Insert(a, it);
                }
                await chatHubContext.Clients.Group(a).SendAsync("GroupCreate", a);
            }
            return Redirect($"/Chat/Chat?t={a}");
        }
        public IActionResult Creategroup(Group obj)
        {
            string us = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string gid = Helper.Groupconv(us);
            int ret = conversationRepository.Add(gid, obj.Convname, obj.ConvDescrip);
            if (ret > 0)
            {
                var groups = obj.Members.Split(';');
                foreach (var m in groups)
                {
                    conversationRepository.Insert(gid,m);
                }
            }
            return Redirect($"/Chat/Chat?t={gid}");
        }
    }
}
