using Microsoft.AspNetCore.Identity;

namespace ControllerManagement.Service
{
    public interface IUserService
    {
        void AddUser(string username, string password);

        void DeleteUser(string username);

        List<string> GetUsers();

        void AddPersonToRole(string username, string role);

        IdentityUser? Authenticate(string username, string password);

        string GenerateJSONWebToken(IdentityUser user);
    }
}
