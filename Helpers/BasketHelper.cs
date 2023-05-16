using System.Data.Common;
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
        public static Dictionary<int, int> GetBasketFromCookie(
            HttpRequest httpRequest,
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                try {
                    Dictionary<int, int> dict_int_int = JsonConvert.DeserializeObject<Dictionary<int, int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);
                    return dict_int_int;
                } catch {
                    SaveToCookies(new Dictionary<int, int> { }, httpResponse, CookieObjectEnum.BasketCookie);
                    return new Dictionary<int, int> { };
                }
            } else {
                SaveToCookies(new Dictionary<int, int> { }, httpResponse, CookieObjectEnum.BasketCookie);
                return new Dictionary<int, int> { };
            }
        }

        public static void UpdateCount(
            int product_id,
            int product_count,
            HttpResponse httpResponse,
            HttpRequest httpRequest
        ){
            Dictionary<int, int> basket = GetBasketFromCookie(httpRequest, httpResponse);
            if (basket.ContainsKey(product_id)){
                basket[product_id] = product_count;
            } else {
                basket.Add(product_id, product_count);
            }
            SaveToCookies(basket, httpResponse, CookieObjectEnum.BasketCookie);
        }

        public static void RestructureBasket(
            Dictionary<int, int> productList,
            HttpResponse httpResponse,
            HttpRequest httpRequest
        ) {
            httpResponse.Cookies.Delete(CookieObjectEnum.BasketCookie.GetName());
            SaveToCookies(productList, httpResponse, CookieObjectEnum.BasketCookie);
        }

        public static void AddToCookieBasket(
            ProductModel product,
            HttpRequest httpRequest, 
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                Dictionary<int, int> basketSaveProducts = JsonConvert.DeserializeObject<Dictionary<int, int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);
                if (basketSaveProducts.ContainsKey(product.Id)) {
                    basketSaveProducts[product.Id] += 1;
                } else {
                    basketSaveProducts.Add(product.Id, 1);
                }
                SaveToCookies(basketSaveProducts, httpResponse, CookieObjectEnum.BasketCookie);
            } else {
                SaveToCookies(new Dictionary<int, int> { { product.Id, 1 } }, httpResponse, CookieObjectEnum.BasketCookie);
            }
        }

        public static void RemoveFromCookieBasket(
            int product_id, 
            HttpRequest httpRequest, 
            HttpResponse httpResponse
        ) {
            if (httpRequest.Cookies.ContainsKey(CookieObjectEnum.BasketCookie.GetName()))
            {
                Dictionary<int, int>  basketSaveProducts = JsonConvert.DeserializeObject<Dictionary<int, int>>(httpRequest.Cookies[CookieObjectEnum.BasketCookie.GetName()]);

                if (basketSaveProducts.ContainsKey(product_id)) {
                    basketSaveProducts.Remove(product_id);                
                }

                SaveToCookies(basketSaveProducts, httpResponse, CookieObjectEnum.BasketCookie);
            } else {
                SaveToCookies(new Dictionary<int, int>  { }, httpResponse, CookieObjectEnum.BasketCookie);
            }
        }

        private static void SaveToCookies (
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