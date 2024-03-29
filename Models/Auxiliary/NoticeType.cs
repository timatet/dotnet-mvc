using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;

namespace dotnet_mvc.Models.Auxiliary
{
    public enum NoticeType
    {
        Default,
        WaitTime,
        DrizzleError,
        AccessError,
        IsAuthorized,
        Error404,
        PasswordReset
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
                case NoticeType.AccessError:
                    return "storm.gif";
                case NoticeType.IsAuthorized:
                    return "isauth.gif";
                case NoticeType.Error404:
                    return "warning.gif";
                case NoticeType.PasswordReset:
                    return "shield.gif";
                default:
                    return "None";
            }
        }
    }
}