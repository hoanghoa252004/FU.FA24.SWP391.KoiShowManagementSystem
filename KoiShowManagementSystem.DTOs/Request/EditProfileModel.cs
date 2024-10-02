namespace KoiShowManagementSystem.DTOs.Request
{
    public class EditProfileModel
    {
        //  Edit password, name, phone number, gender, date of birth ( not  EMAIL ).
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
}
