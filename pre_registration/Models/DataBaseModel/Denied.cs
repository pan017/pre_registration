using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class Denied
    {
        public int id { get; set; }
        public DateTime deniedDate { get; set; }
        public virtual CuponDate CuponDate { get; set; }
        public string reason { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
