using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace dotnet_mvc.Helpers
{
    public static class BasketHelper
    {
        public static List<int> GetBasketFromCookie(
            HttpRequest httpRequest,
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                return JsonConvert.DeserializeObject<List<int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);
            } else {
                SaveToCookies(new List<int> { }, httpResponse, CookieObjectEnum.BasketCookie);
                return new List<int> { };
            }
        }

        public static void RestructureBasket(
            List<int> productList,
            HttpResponse httpResponse,
            HttpRequest httpRequest
        ) {
            httpResponse.Cookies.Delete(CookieObjectEnum.BasketCookie.GetName());
            SaveToCookies(productList, httpResponse, CookieObjectEnum.BasketCookie);
        }

        public static void AddToCookieBasket(
            int product_id,
            HttpRequest httpRequest, 
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                List<int> basketSaveProducts = JsonConvert.DeserializeObject<List<int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);
                basketSaveProducts.Add(product_id);
                SaveToCookies(basketSaveProducts, httpResponse, CookieObjectEnum.BasketCookie);
            } else {
                SaveToCookies(new List<int> { product_id }, httpResponse, CookieObjectEnum.BasketCookie);
            }
        }

        public static void RemoveFromCookieBasket(
            ProductModel productModel, 
            HttpRequest httpRequest, 
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                List<int> basketSaveProducts = JsonConvert.DeserializeObject<List<int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);

                if (basketSaveProducts.Contains(productModel.Id)) {
                    basketSaveProducts.Remove(productModel.Id);                
                }

                SaveToCookies(basketSaveProducts, httpResponse, CookieObjectEnum.BasketCookie);
            } else {
                SaveToCookies(new List<int> { }, httpResponse, CookieObjectEnum.BasketCookie);
            }
        }

        public static void SaveToCookies (
            object Object, 
            HttpResponse httpResponse, 
            CookieObjectEnum cookieObjectEnum
        ) {
            var jsonObject = JsonConvert.SerializeObject(Object);
            httpResponse.Cookies.Append(
                cookieObjectEnum.GetName(), 
                jsonObject,
                new CookieOptions() {
                    MaxAge = cookieObjectEnum.GetTimeSpan()
                }
            );
        }
    }
}