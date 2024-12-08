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
        public UserService(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        private readonly IConfiguration configuration;
        private readonly UserManager<IdentityUser> userManager;

        public void AddUser(string username, string password)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = username,
            };
            
            userManager.CreateAsync(user, password).Wait();
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
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

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
    }
}
