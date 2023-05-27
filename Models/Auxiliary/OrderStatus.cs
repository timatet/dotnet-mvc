using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.Auxiliary
{
    public enum OrderStatus
    {
        [Display(Name = "Оформлен")]
        Created,
        [Display(Name = "Выдан")]
        Issued
    }
}