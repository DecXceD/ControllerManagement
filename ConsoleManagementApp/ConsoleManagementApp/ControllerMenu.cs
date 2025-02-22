using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace ConsoleManagementApp
{
    public class ControllerMenu
    {
        private string? authenticationToken;

        private string? role;

        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7280/api/"),
        };

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

            var response = await client.PostAsync($"User/Login?username={username}&password={password}", null);
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
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authenticationToken}");
        }

        public void Logout()
        {
            authenticationToken = null;
            role = null;
            Console.WriteLine("Logged out");
            client.DefaultRequestHeaders.Remove("Authorization");
        }

        public async Task ShowControllersAsync()
        {
            var response = await client.GetAsync($"Controller/ShowAllControllers");
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

            if (IsAdmin())
            {
                Console.WriteLine("Replace {parameter} {new parameter}");
                Console.WriteLine("Add {parameter} {value}");
                Console.WriteLine("Delete {parameter}");
            }

            Console.WriteLine("Return");

            string[] command = Console.ReadLine().Split();
            switch (command[0])
            {
                case "Update":
                    if (IsWorker() || IsAdmin())
                    {
                        await UpdateParameter(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Replace":
                    if (IsAdmin())
                    {
                        await ReplaceParameter(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Add":
                    if (IsAdmin())
                    {
                        await AddParameter(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Delete":
                    if (IsAdmin())
                    {
                        await DeleteParameter(controllerNumber, command[1]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid Command");
                    break;
            }

        }

        public async Task ShowController(int id)
        {
            var response = await client.GetAsync($"Controller/{id}");
            Controller? content = await response.Content.ReadFromJsonAsync<Controller>();

            if (content != null)
            {
                content.Print();
            }
            else
            {
                Console.WriteLine("Controller not found");
            }
        }

        public async Task UpdateParameter(int id, string name, string value)
        {
            
            var httpContent = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"Controller/UpdateParameter/{id}?name={name}", httpContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Parameter updated");
                await ShowController(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task ReplaceParameter(int id, string name, string newName)
        {
            var response = await client.PatchAsync($"Controller/ReplaceParameter/{id}?name={name}&newName={newName}", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Parameter replaced");
                await ShowController(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task AddParameter(int id, string name, string value)
        {
            var httpContent = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"Controller/AddParameter/{id}?name={name}", httpContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Parameter added");
                await ShowController(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task DeleteParameter(int id, string name)
        {
            var response = await client.PatchAsync($"Controller/DeleteParameter/{id}?name={name}", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Parameter deleted");
                await ShowController(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }
    }
}
