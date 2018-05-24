using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class CuponDate
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int AreaId { get; set; }
        
        public Area Area { get; set; }
    }
}
