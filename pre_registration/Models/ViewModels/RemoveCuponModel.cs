using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class RemoveCuponsModel
    {
        [Display(Name = "Начало периода")]
        public DateTime beginDate { get; set; }
        [Display(Name = "Окончание периода")]
        public DateTime endDate { get; set; }
        [Display(Name = "Район")]
        public int AreaId { get; set; }
        [Display(Name = "Дни недели")]
        public bool[] daysOfWeek { get; set; }
    }
}
