using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Services
{
    public class AppConfig
    {
        public Logging Logging { get; set; }
        public NotificationEmail NotificationEmail { get; set; }

    }
    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
        
    }
    public class LogLevel
    {
        public string Default { get; set; }
    }
    public class NotificationEmail
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string DisplayngName { get; set; }
    }
}
