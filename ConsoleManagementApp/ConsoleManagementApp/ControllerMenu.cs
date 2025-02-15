using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ConsoleManagementApp
{
    public class ControllerMenu
    {
        private string? authenticationToken;

        private string? role;

        public bool IsWorker()
        {
            return role != null && role.ToLower() == "worker";
        }

        public bool IsAdmin()
        {
            return role != null && role.ToLower() == "admin";
        }

        public async Task LoginAsync()
        {
            Console.WriteLine("Enter Username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();
            HttpClient client = new HttpClient();

            var response = await client.PostAsync($"https://localhost:7280/api/User/Login?username={username}&password={password}", null);
            var content = await response.Content.ReadAsStringAsync();
            var contentDict = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

            if (contentDict == null || !contentDict.ContainsKey("token"))
            {
                Console.WriteLine("Server Error");
                return;
            }

            authenticationToken = contentDict["token"];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(authenticationToken);
            role = jwtSecurityToken.Claims.First(claim => claim.Type.ToLower().EndsWith("role")).Value;
        }

        public void Logout()
        {
            authenticationToken = null;
            role = null;
            Console.WriteLine("Logged out");
        }

        public async Task ShowControllersAsync()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:7280/api/Controller/ShowAllControllers");
            var content = await response.Content.ReadAsStringAsync();
            List<string>? result = JsonSerializer.Deserialize<List<string>>(content);

            if (result == null || !result.Any())
            {
                Console.WriteLine("No controllers found");
                return;
            }

            foreach (string line in result)
            {
                Console.WriteLine(line);
            }
        }

        public async Task Edit(int controllerNumber)
        {
            Console.WriteLine("Update {parameter} {value}");
            Console.WriteLine("Return");

            if (IsAdmin())
            {
                Console.WriteLine("Replace");
                Console.WriteLine("Add");
                Console.WriteLine("Delete");
            }
        }
    }
}
