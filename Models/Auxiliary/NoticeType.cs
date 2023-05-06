using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;

namespace dotnet_mvc.Models.Auxiliary
{
    public enum NoticeType
    {
        Default,
        WaitTime,
        DrizzleError
    }

    public static class NoticeTypeExtensions
    {
        public static string GetValue(this NoticeType noticeType)
        {
            switch (noticeType)
            {
                case NoticeType.Default:
                    return "";
                case NoticeType.WaitTime:
                    return "waittime.gif";
                case NoticeType.DrizzleError:
                    return "drizzle.gif";
                default:
                    return "None";
            }
        }
    }
}