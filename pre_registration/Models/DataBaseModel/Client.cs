using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pre_registration.Models.ViewModels;
namespace pre_registration.Models
{
    public class Client
    {
        public int id { get; set; }
       
        public string Comment { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? UserDataID { get; set; }
        public UserData UserData { get; set; }
    }
}
