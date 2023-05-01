using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace dotnet_mvc.Models.DataModels
{
    [Table("ProductCharacteristic")]
    public class ProductCharacteristic
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Вес", ShortName = "Weight")]
        public string Weight { get; set; }

        [Display(Name = "Материал", ShortName = "Material")]
        public string Material { get; set; }

        [Display(Name = "Тип товара", ShortName = "ProductType")]
        public string ProductType { get; set; }

        [Display(Name = "Пол", ShortName = "UserGender")]
        public string UserGender { get; set; }

        [Display(Name = "Сезон применения", ShortName = "UsingSeason")]
        public string UsingSeason { get; set; }

        [Display(Name = "Влагозащита", ShortName = "MoistureProtection")]
        public string MoistureProtection { get; set; }

        [Display(Name = "Прочность", ShortName = "ImpactProtection")]
        public string ImpactProtection { get; set; }

        [Display(Name = "Мембрана", ShortName = "PresenceOfMembrane")]
        public string PresenceOfMembrane { get; set; }

        public void SetFields(List<string> fieldNamesAndValues) 
        {
            foreach (string nameAndValue in fieldNamesAndValues) 
            {
                string[] nameAndValueParsed = nameAndValue.Split('&');
                switch (nameAndValueParsed[0]) {
                    case "Weight": this.Weight = nameAndValueParsed[1]; break;
                    case "Material": this.Material = nameAndValueParsed[1]; break;
                    case "ProductType": this.ProductType = nameAndValueParsed[1]; break;
                    case "UserGender": this.UserGender = nameAndValueParsed[1]; break;
                    case "UsingSeason": this.UsingSeason = nameAndValueParsed[1]; break;
                    case "MoistureProtection": this.MoistureProtection = nameAndValueParsed[1]; break;
                    case "ImpactProtection": this.ImpactProtection = nameAndValueParsed[1]; break;
                    case "PresenceOfMembrane": this.PresenceOfMembrane = nameAndValueParsed[1]; break;
                }
            }
        }

        public static IEnumerable<DisplayAttribute> GetAttributesNames()
        {
            return typeof(ProductCharacteristic)
                .GetProperties()
                .SelectMany(x => x.GetCustomAttributes(typeof(DisplayAttribute), true)
                .Select(e => ((DisplayAttribute)e)))
                .Where(x => x != null).Select(x => x);
        }

        public Dictionary<string, string> GetDisplaysAndValues()
        {
            Dictionary<string, string> DisplaysAndValues = new Dictionary<string, string>();
            var prop = this.GetType().GetProperties();
            foreach (var p in prop) {
                var propertyValue = p.GetValue(this);
                var propertyName = (p.GetCustomAttributes(typeof(DisplayAttribute), true)
                    .FirstOrDefault() as DisplayAttribute);

                if (propertyValue != null && propertyName != null) {
                    DisplaysAndValues.Add(propertyName.Name, propertyValue.ToString());
                }
            }

            return DisplaysAndValues;
        }

        public Dictionary<string, string> GetDisplaysAndValuesPresent()
        {
            Dictionary<string, string> DisplaysAndValues = new Dictionary<string, string>();
            var prop = this.GetType().GetProperties();
            foreach (var p in prop) {
                var propertyValue = p.GetValue(this);
                var propertyName = (p.GetCustomAttributes(typeof(DisplayAttribute), true)
                    .FirstOrDefault() as DisplayAttribute);

                if (propertyValue != null && propertyName != null) {
                    DisplaysAndValues.Add(string.Format("{0} ({1})", propertyName.Name, propertyValue.ToString()), 
                    string.Format("{0}&{1}", propertyName.GetShortName(), propertyValue.ToString()));
                }
            }

            return DisplaysAndValues;
        }

    }
}