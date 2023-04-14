﻿using Chatbox;
using ChatBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Globalization;
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
        public IActionResult Index()
        {            
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var convid = messageRepository.GetGroups(userid);
            if(convid.Count() > 0)
            {
                return Redirect($"/Chat/Chat?t={convid.First().ConvId}");
            }
            return Redirect($"/Chat/Chat");
        }
        [ServiceFilter(typeof(ContactFilter))]
        public IActionResult Chat(string t="")
        {
            ViewBag.conv = t;
            if (!string.IsNullOrEmpty(t))
            {
                string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string name = conversationRepository.GetNameById(t);
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
                
                ViewBag.chatname = name;
            
                ViewBag.messages = messageRepository.GetMessages(t);
            }
            
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
            string convid = Helper.StringConv(us, i);
            List<string> group = new List<string>();
            List<string> convname = new List<string>();
            group.Add(i);
            group.Add(us);
            int ret = conversationRepository.Add(convid, "", "");
            if (ret > 0)
            {
                foreach (var it in group)
                {
                    conversationRepository.Insert(convid, it);
                }
            }
            string name = conversationRepository.GetNameById(convid);
            if (string.IsNullOrEmpty(name))
            {
                foreach (var user in group)
                {
                    convname.Add(conversationRepository.GetMembersInGroup(user,convid).ToString());
                }
            }
            else
            {
                convname.Add(name);
            }
            var a = memberRepository.GetMemberById(us);
            ConversationInfo obj = new ConversationInfo
            {
                ConvId = convid,
                Avatar = a.Avatar,
                LastTimeActive = a.LastTimeActive,
                MessageDate = DateTime.ParseExact(DateTime.Now.ToString("hh:mm tt"), "hh:mm tt", CultureInfo.InvariantCulture),
            };
            //await chatHubContext.Clients.Groups(us).SendAsync("Creategroup", obj, convname, us);
            await chatHubContext.Clients.Groups(i).SendAsync("Creategroup", obj,convname,us);
            return Redirect($"/Chat/Chat?t={convid}");
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
