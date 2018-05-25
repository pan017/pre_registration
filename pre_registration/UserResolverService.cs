//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace pre_registration
//{
//    public class UserResolverService
//    {
//        private readonly IHttpContextAccessor _context;
//        public UserResolverService(IHttpContextAccessor context)
//        {
//            _context = context;
//        }

//        public string GetUser()
//        {
//            return  _context.HttpContext.User?.Identity?.Name;
//        }
//    }
//}
