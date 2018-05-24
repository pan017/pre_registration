using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class MyCuponsViewModel
    {
        public int id { get; set; }
        public string orderDate { get; set; }
        public string cuponDate { get; set; }
        public string cuponTime { get; set; }

        public string Coment { get; set; }
        public string DeniedKey { get; set; }
    }
}
