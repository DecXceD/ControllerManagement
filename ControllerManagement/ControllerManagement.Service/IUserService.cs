using Microsoft.AspNetCore.Identity;

namespace ControllerManagement.Service
{
    public interface IUserService
    {
        void AddUser(string username, string password);

        void DeleteUser(string username);

        List<string> GetUsers();

        IdentityUser? Authenticate(string username, string password);

        string GenerateJSONWebToken(IdentityUser user);
    }
}
