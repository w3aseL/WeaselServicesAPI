using DataAccessLayer;
using DataAccessLayer.Models;
using WeaselServicesAPI.Exceptions;
using WeaselServicesAPI.Helpers;
using WeaselServicesAPI.Helpers.Interfaces;
using WeaselServicesAPI.Helpers.JWT;
using WeaselServicesAPI.Models;

namespace WeaselServicesAPI.Services
{
    public class UserService
    {
        private readonly ServicesAPIContext _ctx;
        private readonly IEmailSender _emailSender;
        private readonly ITokenGenerator _jwtGenerator;

        public UserService(ServicesAPIContext ctx, IEmailSender sender, ITokenGenerator tokenGen) {
            _ctx = ctx;
            _emailSender = sender;
            _jwtGenerator = tokenGen;
        }

        public User RegisterNewUser(string username, string email)
        {
            if (_ctx.Users.Any() && _ctx.Users.Where(u => u.Username == username || u.Email == email).FirstOrDefault() != null)
                throw new UserExistsException("User found with email or username.");

            var user = new User()
            {
                Username = username,
                Email = email
            };

            _ctx.Users.Add(user);
            _ctx.SaveChanges();

            var userReq = new UserAccountRequest()
            {
                UserId = user.UserId,
                IsRegistrationRequest = true
            };
                
            _ctx.UserAccountRequests.Add(userReq);
            _ctx.SaveChanges();

            // TODO: Send email w/ custom message
            if(!string.IsNullOrEmpty(email))
            {
                _emailSender.SendEmail(new Message(new string[]{ email }, "Registration for Weasel's Dashboard",
                    $"Registration code is \"{ userReq.RequestCode }\".<br/>Register at <insert URL here>."));
            }

            return user;
        }

        public bool ConfirmUserRegistration(string reqCode, string pass)
        {
            // fetch user request from request code
            var req = _ctx.UserAccountRequests.Where(r => r.RequestCode.ToString().Equals(reqCode)).FirstOrDefault();
            if (req is null)
                throw new UserNotFoundException("Request for user account registration/modification not found.");

            // get user
            var user = _ctx.Users.Where(u => u.UserId == req.UserId).FirstOrDefault();
            if (user is null)
                throw new UserNotFoundException("Could not find user linked to request.");

            // get salt and hash password
            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(pass, salt);

            // Update user with password info
            user.Password = hashedPassword;
            user.PasswordSeed = salt;
            _ctx.SaveChanges();

            // remove request since it is completed
            _ctx.UserAccountRequests.Remove(req);
            _ctx.SaveChanges();

            return true;
        }

        public LoginResponseModel LoginUser(string username, string password)
        {
            // fetch user by username
            var user = _ctx.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user is null)
                throw new UserNotFoundException("Could not find user with that username!");

            // check if user is fully registered
            if (_ctx.UserAccountRequests.Where(r => r.UserId == user.UserId && r.IsRegistrationRequest).Any())
                throw new UserNotFoundException("User is required to finish the registration process.");

            // check password
            if (!PasswordHasher.ComparePasswords(user.Password, PasswordHasher.HashPassword(password, user.PasswordSeed)))
                throw new UserNotFoundException("The password provided is not correct!");

            // generate access token
            var access = _jwtGenerator.GenerateToken(user, TokenType.Access);

            // generate refresh token
            var refresh = _jwtGenerator.GenerateToken(user, TokenType.Refresh);

            return new LoginResponseModel
            {
                AccessToken = access,
                RefreshToken = refresh,
                User = new UserModel(user)
            };
        }
    }
}
