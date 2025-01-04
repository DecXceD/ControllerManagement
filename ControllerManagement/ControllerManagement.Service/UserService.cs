using ControllerManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControllerManagement.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(IConfiguration configuration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void AddUser(string username, string password)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = username,
            };
            
            var task = userManager.CreateAsync(user, password);
            task.Wait();

            if(!task.Result.Succeeded)
            {
                throw new ApplicationException(task.Result.Errors.First().Description);
            }

            if (!roleManager.RoleExistsAsync(UserRoles.User).Result)
            {
                roleManager.CreateAsync(new IdentityRole(UserRoles.User)).Wait();
            }

            userManager.AddToRoleAsync(user, UserRoles.User).Wait();

        }

        public IdentityUser? Authenticate(string username, string password)
        {
            var user = userManager.FindByNameAsync(username).Result;
            if (user != null && userManager.CheckPasswordAsync(user, password).Result)
            {
                return user;
            }

            return null;
        }

        public void DeleteUser(string username)
        {
            var user = userManager.FindByNameAsync(username).Result ?? throw new ArgumentException("Username not found");
            userManager.DeleteAsync(user).Wait();
        }

        public string GenerateJSONWebToken(IdentityUser user)
        {
            var userRoles = userManager.GetRolesAsync(user).Result;
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string jwtSecret = this.configuration["JWT:Secret"]!;
            byte[] jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSecret);
            var authSigningKey = new SymmetricSecurityKey(jwtSecretBytes);
            var token = new JwtSecurityToken(
                issuer: this.configuration["JWT:ValidIssuer"],
                audience: this.configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public List<string> GetUsers()
        {
            return userManager.Users.Select(x => x.UserName!).ToList();
        }

        public void AddPersonToRole(string username, string role)
        {
            var user = userManager.FindByNameAsync(username).Result ?? throw new ArgumentException("Wrong username");

            if (role != UserRoles.User && role != UserRoles.Worker && role != UserRoles.Admin)
            {
                throw new ArgumentException("Invalid role");
            }

            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }

            IList<string> roleList = userManager.GetRolesAsync(user).Result;
            userManager.RemoveFromRolesAsync(user, roleList).Wait();
            userManager.AddToRoleAsync(user, role).Wait();
        }
    }
}
