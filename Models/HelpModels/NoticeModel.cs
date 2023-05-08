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
        public string ReturnToBackUrl { get; set; }

        public string GetNoticeValue() => NoticeType.GetValue();

        public static NoticeModel GetAccessErrorNoticeModel() => 
            new NoticeModel(
                NoticeType.AccessError, 
                "Ошибка доступа",
                "Недостаточно прав для просмотра данной страницы." +
                "Если вы не авторизованы, авторизуйтесь. Если проблема " +
                "не решится, свяжитесь с администрацией сайта."
            );

        public NoticeModel(
            NoticeType noticeType, 
            string messageHeader, 
            string messageDescription
        ){
            this.NoticeType = noticeType;
            this.MessageDescription = messageDescription;
            this.MessageHeader = messageHeader;
        }

        public NoticeModel(
            NoticeType noticeType, 
            string messageHeader, 
            string messageDescription,
            string returnToBackUrl
        ){
            this.NoticeType = noticeType;
            this.MessageDescription = messageDescription;
            this.MessageHeader = messageHeader;
            this.ReturnToBackUrl = returnToBackUrl;
        }

    }
}