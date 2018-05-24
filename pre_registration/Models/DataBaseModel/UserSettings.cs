using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.DataBaseModel
{
    public class UserSettings
    {
        public int id { get; set; }
        public bool SendEmail { get; set; }
        public bool SendReminder { get; set; }

    }
}
