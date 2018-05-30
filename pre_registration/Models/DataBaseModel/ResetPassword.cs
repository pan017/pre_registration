using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.DataBaseModel
{
    public class ResetPassword
    {
        public int id { get; set; }
        public string UniqueKey { get; set; }
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
