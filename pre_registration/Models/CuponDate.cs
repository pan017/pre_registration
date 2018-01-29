using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class CuponDate
    {
        public Guid id { get; set; }
        public DateTime date { get; set; }
        public string Status { get; set; }
        public virtual Client Client { get; set; }
        public virtual Area Area { get; set; }
        public DateTime? regDate { get; set; }
        //специалист
        // процедура

    }
}
