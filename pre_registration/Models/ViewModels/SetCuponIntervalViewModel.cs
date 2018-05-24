using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class SetCuponIntervalViewModel
    {
        [Display(Name = "Начало периода")]
        public DateTime beginDate { get; set; }
        [Display(Name = "Окончание периода")]
        public DateTime endDate { get; set; }
        [Display(Name = "Часы")]
        public int beginTimeHours { get; set; }
        [Display(Name = "Мин.")]
        public int beginTimeMins { get; set; }
        [Display(Name = "Часы")]
        public int endTimeHours { get; set; }
        [Display(Name = "Мин.")]
        public int endTimeMins { get; set; }
        [Display(Name = "Интервал, мин.")]
        public int interval { get; set; }
        [Display(Name = "Район")]
        public int AreaId { get; set; }
        [Display(Name = "Дни недели")]
        public bool[] daysOfWeek { get; set; }
    }
}
