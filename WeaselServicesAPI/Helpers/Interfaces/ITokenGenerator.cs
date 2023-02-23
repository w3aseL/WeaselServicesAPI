using DataAccessLayer.Models;
using WeaselServicesAPI.Helpers.JWT;

namespace WeaselServicesAPI.Helpers.Interfaces
{
    public interface ITokenGenerator
    {
        public string GenerateToken(User u, TokenType type);
        public bool ValidateToken(string token, out string uuid);
    }
}
