using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;

namespace dotnet_mvc.Models.Auxiliary
{
    public enum CookieObjectEnum
    {
        BasketCookie
    }

    public static class CookieObjectValueExtension
    {
        public static string GetName(this CookieObjectEnum cookieObjectEnum)
        {
            switch (cookieObjectEnum)
            {
                case CookieObjectEnum.BasketCookie:
                    return "BasketSaveProducts";
                default:
                    return "None";
            }
        }

        public static TimeSpan GetTimeSpan(this CookieObjectEnum cookieObjectEnum)
        {
            switch (cookieObjectEnum)
            {
                case CookieObjectEnum.BasketCookie:
                    return new TimeSpan(30, 0, 0, 0);
                default:
                    return new TimeSpan(0, 1, 0, 0);
            }
        }
    }
}