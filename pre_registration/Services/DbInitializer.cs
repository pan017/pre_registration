using pre_registration.Models;
using pre_registration.Models.DataBaseModel;
using System.Linq;

namespace pre_registration.Services
{
    public class DbInitializer
    {
        public static async void Initialize(ApplicationContext context)
        {
           if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new ApplicationRole { Name = "user" },
                    new ApplicationRole { Name = "superuser" },
                    new ApplicationRole { Name = "admin" });
                context.SaveChanges();

            }
           if (!context.Users.Any())
            {
                UserService.CreateUser(new Models.ViewModels.AddUserViewModel
                {
                    FirstName = "admin",
                    LastName = "admin",
                    SecondName = "admin",
                    Login = "notification.prereg@mgaon.by",
                    Password = "4rfvBGT5",
                    PasswordConfirm = "4rfvBGT5",
                    Phone = "+375171234567",
                    RoleId = context.Roles.FirstOrDefault(x => x.Name == "admin").Id                    
                }, context);
                UserService.CreateUser(new Models.ViewModels.AddUserViewModel
                {
                    FirstName = "Илья",
                    LastName = "Паныш",
                    SecondName = "Сергеевич",
                    Login = "pan017@yandex.by",
                    Password = "a2l6e3x2IS",
                    PasswordConfirm = "a2l6e3x2IS",
                    Phone = "+375291849927",
                    RoleId = context.Roles.FirstOrDefault(x => x.Name == "admin").Id
                }, context);
            }
           if (!context.Areas.Any())
            {
                context.Areas.AddRange(
                    new Area { Name = "Мингорисполком", Adres = "пр-т Независимости 8", email = "mgik.okno5@minsk.gov.by", NotificationEmail = "mgik.okno5@minsk.gov.by", Phone = "+375 17 2180194, +375 17 2000460, +375 17 2180088", website = "https://minsk.gov.by/", WorkTime = "", MapUrl = "https://yandex.ua/map-widget/v1/-/CBuq6Qgd~A" },
                    new Area { Name = "Заводской район", Adres = "ул. Жилуновича 17", email = "zav.okno@minsk.gov.by", NotificationEmail = "zav.okno@minsk.gov.by", Phone = "+375 17 3892684", website = "http://www.zav.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6Fxb3D" },
                    new Area { Name = "Ленинский район", Adres = "ул. Маяковского 83", email = "", NotificationEmail = "", Phone = "+375 17 2239906, +375 17 2239907, +375 17 2230027, +375 17 2235808", website = "http://www.lenadmin.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuqZXsQ-D" },
                    new Area { Name = "Московский район", Adres = "пр-т Дзержинского 10", email = "mos.okno@minsk.gov.by", NotificationEmail = "mos.okno@minsk.gov.by", Phone = "+375 17 2009463, +375 17 2000593, +375 17 2006626", website = "http://www.mosk.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuqZ2wblD" },
                    new Area { Name = "Октябрьский район", Adres = "ул. Чкалова 6", email = "okt.okno@minsk.gov.by", NotificationEmail = "okt.okno@minsk.gov.by", Phone = "+375 17 2054190, +375 17 2078517", website = "http://www.okt.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuqZ-Sg2A" },
                    new Area { Name = "Партизанский район", Adres = "ул. Захарова 53", email = "prt.okno@minsk.gov.by", NotificationEmail = "prt.okno@minsk.gov.by", Phone = "+375 17 2941846, +375 17 2941850", website = "http://www.part.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6ARLpD" },
                    new Area { Name = "Первомайский район", Adres = "пер. Чорного К. 5", email = "prv.okno@minsk.gov.by", NotificationEmail = "prv.okno@minsk.gov.by", Phone = "+375 17 2807589, +375 17 2807584", website = "http://www.perv.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6EqFGA" },
                    new Area { Name = "Советский район", Adres = "ул. Дорошевича 8", email = "sov.okno@minsk.gov.by", NotificationEmail = "sov.okno@minsk.gov.by", Phone = "+375 17 3317474", website = "http://www.sov.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6IuWCC" },
                    new Area { Name = "Фрунзенский район", Adres = "ул. Кальварийская 39", email = "frn.okno@minsk.gov.by", NotificationEmail = "frn.okno@minsk.gov.by", Phone = "+375 17 2047830, +375 17 2549263, +375 17 2169310, +375 17 2047806", website = "http://www.fr.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6IxTPD" },
                    new Area { Name = "Центральный район", Adres = "ул. Мельникайте 6", email = "cen.okno@minsk.gov.by", NotificationEmail = "cen.okno@minsk.gov.by", Phone = "+375 17 2030130, +375 17 2030145, +375 17 2030151, +375 17 3064002, +375 17 3060840", website = "http://centr.minsk.gov.by", WorkTime = "", MapUrl = "https://yandex.by/map-widget/v1/-/CBuq6MHOtC" });
                context.SaveChanges();
            }
        }
    }
}
