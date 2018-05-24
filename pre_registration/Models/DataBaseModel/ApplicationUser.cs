using pre_registration.Models.DataBaseModel;


namespace pre_registration.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public bool confirmedEmail { get; set; }
        public string confirmKey { get; set; }

        public int? UserSettingsId { get; set; }
        public UserSettings UserSettings { get; set; }

        public int? RoleId { get; set; }
        public ApplicationRole Role { get; set; }

        public int? AreaId { get; set; }
        public Area Area { get; set; }
        
        public int UserDataID { get; set; }
        public UserData UserData { get; set; }
    }

    
}
