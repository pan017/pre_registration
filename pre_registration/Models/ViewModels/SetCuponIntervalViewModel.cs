using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class SetCuponIntervalViewModel: RemoveCuponsModel
    {
        
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
       
    }
}
