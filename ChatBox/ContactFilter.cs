﻿using ChatBox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ChatBox
{
    public class ContactFilter : IActionFilter
    {
        IMemberRepository memberRepository;
        IMessageRepository messageRepository;
        IConversationRepository conversationMemberRepository;
        
        public ContactFilter(IMemberRepository memberRepository, IMessageRepository messageRepository, IConversationRepository conversationMemberRepository)
        {
            this.memberRepository = memberRepository;
            this.messageRepository = messageRepository;
            this.conversationMemberRepository = conversationMemberRepository;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is Controller con)
            {                
                string id = con.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id != null)
                {
                    List<string> lstUsr2 = new List<string>();
                    List<string> lstConv = new List<string>();
                    IEnumerable<ConversationInfo> list = messageRepository.GetDirects(id);
                    IEnumerable<GroupInfo> listG = messageRepository.GetGroups(id);
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item.Convname))
                        {
                            item.Convname = conversationMemberRepository.GetMembersInGroup(id, item.ConvId);
                        }
                        lstUsr2.Add(item.UserId);
                        lstConv.Add(item.ConvId);
                    }
                    foreach (var item in listG)
                    {
                        if (string.IsNullOrEmpty(item.Avatar))
                        {
                            item.Avatar = "no-image.jpg";
                        }
                        item.Content= item.Content == null ? "" : $"{item.Content}";
                        item.CountMessage = item.CountMessage == null ? 0 : item.CountMessage;
                    }
                    con.ViewBag.usrDetail = memberRepository.GetMemberById(id).Avatar;
                    con.ViewData["GroupChat"] = listG;
                    con.ViewData["directChats"]=list;
                    IEnumerable<Member> users = memberRepository.GetMembersById(id);
                    con.ViewData["users"] = users;
                    List<string> lstUsr = new List<string>();
                    foreach (var it in users)
                    {
                        lstUsr.Add(it.UserId.ToString());
                    }
                    con.ViewBag.allMem = string.Join(";",lstUsr);
                    con.ViewBag.allMem2 = string.Join(";",lstUsr2);
                }                
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}
