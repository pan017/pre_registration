﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.DataBaseModel
{
    public class SentNotification
    {
        public int id { get; set; }
        public int OrderId { get; set; }
        public virtual Order  Order{ get; set; }
        public bool isSent { get; set; }

    }
}
