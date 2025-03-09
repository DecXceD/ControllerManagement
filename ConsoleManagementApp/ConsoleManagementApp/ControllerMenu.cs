using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata;
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

        public async Task EditAsync(int controllerNumber)
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
                        await UpdateParameterAsync(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Replace":
                    if (IsAdmin())
                    {
                        await ReplaceParameterAsync(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Add":
                    if (IsAdmin())
                    {
                        await AddParameterAsync(controllerNumber, command[1], command[2]);
                    }
                    else
                    {
                        Console.WriteLine("You don't have permissions for the action");
                    }
                    break;

                case "Delete":
                    if (IsAdmin())
                    {
                        await DeleteParameterAsync(controllerNumber, command[1]);
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

        public async Task ShowControllerAsync(int id)
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

        public async Task UpdateParameterAsync(int id, string name, string value)
        {
            
            var httpContent = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"Controller/UpdateParameter/{id}?name={name}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter updated");
                await ShowControllerAsync(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task ReplaceParameterAsync(int id, string name, string newName)
        {
            var response = await client.PatchAsync($"Controller/ReplaceParameter/{id}?name={name}&newName={newName}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter replaced");
                await ShowControllerAsync(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task AddParameterAsync(int id, string name, string value)
        {
            var httpContent = new StringContent(value, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"Controller/AddParameter/{id}?name={name}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter added");
                await ShowControllerAsync(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task DeleteParameterAsync(int id, string name)
        {
            var response = await client.PatchAsync($"Controller/DeleteParameter/{id}?name={name}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter deleted");
                await ShowControllerAsync(id);
            }
            else
            {
                Console.WriteLine("Parameter not found");
            }
        }

        public async Task ShowUsersAsync()
        {
            var response = await client.GetAsync($"User/GetUsers/");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();           
                List<string>? result = JsonSerializer.Deserialize<List<string>>(content);
                Console.WriteLine("Users:");

                foreach(string user in result!)
                {
                    Console.WriteLine(user);
                }
            }
            else
            {
                Console.WriteLine("You don't have permissions for the action");
            }
        }

        public async Task AddUserAsync(string username, string password, string role)
        {
            var response = await client.PostAsync($"User/AddUser?username={username}&password={password}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User added");

                response = await client.PostAsync($"User/AddPersonToRole?username={username}&role={role}", null);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Invalid role");
                }
            }
            else
            {
                Console.WriteLine("Couldn't add user");
            }
        }

        public async Task DeleteUserAsync(string username)
        {
            var response = await client.DeleteAsync($"User/DeleteUser?username={username}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User deleted");
            }
            else
            {
                Console.WriteLine("Couldn't delete user");
            }
        }
    }
}
