using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.Auxiliary
{
  public enum CategoryEnum
    {
        [Display(Name = "Спорт")]
        Sport,
        [Display(Name = "Горный туризм")]
        MountainTourism,
        [Display(Name = "Трекинг")]
        Tracking,
        [Display(Name = "Автотуризм")]
        AutoTourism,
        [Display(Name = "Альпинизм")]
        Mountaineering,
        [Display(Name = "Водный туризм")]
        WaterTourism,
        [Display(Name = "Горные лыжи")]
        MountainSki,
        [Display(Name = "Конный спорт")]
        HorseSport
    }
}