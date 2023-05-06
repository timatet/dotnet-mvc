using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using Microsoft.AspNetCore.Http;

namespace dotnet_mvc.Models.HelpModels
{
    public class NoticeModel
    {
        public NoticeType NoticeType { get; set; } = NoticeType.Default;
        public string MessageHeader { get; set; }
        public string MessageDescription { get; set; }

        public string GetNoticeValue() => NoticeType.GetValue();

        public NoticeModel(
            NoticeType noticeType, 
            string messageHeader, 
            string messageDescription
        ){
            this.NoticeType = noticeType;
            this.MessageDescription = messageDescription;
            this.MessageHeader = messageHeader;
        }

    }
}