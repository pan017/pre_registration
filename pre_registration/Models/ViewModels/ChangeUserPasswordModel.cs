using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class ChangeUserPasswordModel
    {
        public string id { get; set; }  
        public string userName { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
    }
}
