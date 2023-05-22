using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.DataModels;

namespace dotnet_mvc.Helpers
{
    public static class HtmlGenerator
    {
        public static string GetBasketHtml(
            Dictionary<ProductModel, int> productsLinks
        ) {
            StringBuilder stringBuilder = new StringBuilder();

            double total = 0;

            foreach (var productsLink in productsLinks) {
                stringBuilder.Append("<b>" + productsLink.Key.Name + "</b><br>");
                stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;Размер: " + productsLink.Key.ProductCharacteristic.Size + "<br>");
                stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;Цвет: " + productsLink.Key.ProductCharacteristic.Color + "<br>");
                stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;Количество в корзине: " + productsLink.Value + "&nbsp;шт.<br>");
                stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;Стоимость: " + productsLink.Value * productsLink.Key.Cost+ "&nbsp;₽<br>");
                stringBuilder.Append("<hr>");
                total += productsLink.Value * productsLink.Key.Cost;
            }

            stringBuilder.Append("<h3>ИТОГ: " + total + "&nbsp;₽</h3>");

            return stringBuilder.ToString();
        }
    }
}