using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class SetCuponIntervalViewModel
    {
        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }
        [Display(Name = "Часы")]
        public int beginTimeHours { get; set; }
        [Display(Name = "Мин.")]
        public int beginTimeMins { get; set; }
        [Display(Name = "Часы")]
        public int endTimeHours { get; set; }
        [Display(Name = "Мин.")]
        public int endTimeMins { get; set; }
        public int interval { get; set; }
        public string AreaId { get; set; }
        public bool[] daysOfWeek { get; set; }
    }
}
