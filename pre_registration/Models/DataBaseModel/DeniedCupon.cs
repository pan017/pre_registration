using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.DataBaseModel
{
    public class DeniedCupon
    {
        public int id { get; set; }
        public int OrderId { get; set; }
        public string DeniedKey { get; set; }
        public Order Order { get; set; }

        public DeniedCupon() { }

        public DeniedCupon(int cuponId, string key)
        {
            OrderId = cuponId;
            DeniedKey = key;
        }
    }
}
