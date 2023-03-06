using DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace WeaselServicesAPI
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
    }

    public class ConfirmRegistrationModel
    {
        [Required]
        public string RequestCode { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserModel
    {
        public string UUID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public UserModel() { }

        public UserModel(User u)
        {
            UUID = u.Uuid.ToString();
            Username = u.Username;
            Email = u.Email;
        }
    }

    public class LoginResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserModel User { get; set; }
    }

    public class RefreshModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
