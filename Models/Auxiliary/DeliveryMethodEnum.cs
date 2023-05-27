using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.Auxiliary
{
    public enum DeliveryMethodEnum
    {
        [Display(Name = "Самовывоз со склада")]
        PickUpFromStore,
        [Display(Name = "Курьер")]
        Courier,
        [Display(Name = "Пункт выдачи заказов")]
        PickUpPoint
    }
}