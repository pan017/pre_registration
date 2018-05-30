using pre_registration.Models;
using pre_registration.Models.DataBaseModel;
using pre_registration.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration
{
    public class UserService
    {
        public static ApplicationUser CreateUser(AddUserViewModel model, ApplicationContext db)
        {
            UserData userData = new UserData();
            userData.EmailAdress = model.Login;
            userData.FirstName = model.FirstName;
            userData.LastName = model.LastName;
            userData.SecondName = model.SecondName;
            userData.Phone = model.Phone;
            db.UsersData.Add(userData);
            db.SaveChanges();
            UserSettings userSetings = new UserSettings()
            {
                SendEmail = true,
                SendReminder = true
            };
            db.UserSettings.Add(userSetings);
            db.SaveChanges();
            ApplicationUser newUser = new ApplicationUser {
                Login = model.Login,
                Password = model.Password,
                UserDataID = userData.id,
                confirmedEmail = false,
                confirmKey = Helpers.ConvertStringtoMD5(Guid.NewGuid().ToString()),
                UserSettingsId = userSetings.id
            };
            ApplicationRole userRole = db.Roles.FirstOrDefault(x => x.Id == model.RoleId);

            if (userRole != null)
                newUser.RoleId = userRole.Id;
            Area userArea = db.Areas.FirstOrDefault(x => x.Id == model.AreaID);
            if (userArea != null)
                newUser.AreaId = userArea.Id;
           
            db.Users.Add(newUser);
            db.SaveChanges();
            return newUser;
        }
        
    }
}
