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

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Username or password is incorrect.");
                return;
            }

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

        public async Task EditAsync(int id)
        {
            while (true)
            {
                if (await ShowControllerAsync(id) == false)
                {
                    break;
                }

                Console.WriteLine("Update {parameter} {value}");

                if (IsAdmin())
                {
                    Console.WriteLine("Rename {parameter} {new parameter}");
                    Console.WriteLine("Add {parameter}");
                    Console.WriteLine("Delete {parameter}");
                }

                Console.WriteLine("Return");
                Console.WriteLine();

                string[] command = Console.ReadLine().Split();
                switch (command[0])
                {
                    case "Update":
                        if (IsWorker() || IsAdmin())
                        {
                            await UpdateParameterAsync(id, command[1], command[2]);
                        }
                        else
                        {
                            Console.WriteLine("You don't have permissions for the action");
                        }
                        break;

                    case "Rename":
                        if (IsAdmin())
                        {
                            await RenameParameterAsync(id, command[1], command[2]);
                        }
                        else
                        {
                            Console.WriteLine("You don't have permissions for the action");
                        }
                        break;

                    case "Add":
                        if (IsAdmin())
                        {
                            Console.WriteLine("Enter id");
                            int parameterId = int.Parse(Console.ReadLine());

                            Console.WriteLine("Enter value");
                            double value = double.Parse(Console.ReadLine());

                            Console.WriteLine("Enter Min Value");
                            double minValue = double.Parse(Console.ReadLine());

                            Console.WriteLine("Enter Max Value");
                            double maxValue = double.Parse(Console.ReadLine());

                            Console.WriteLine("Is the parameter a constant? 0 = no, 1 = yes");
                            bool isConstant = Console.ReadLine() == "1";

                            Console.WriteLine("Is the parameter an integer? 0 = no, 1 = yes");
                            bool isInt = Console.ReadLine() == "1";

                            await AddParameterAsync(id, command[1], parameterId, value, minValue, maxValue, isConstant, isInt);
                        }
                        else
                        {
                            Console.WriteLine("You don't have permissions for the action");
                        }
                        break;

                    case "Delete":
                        if (IsAdmin())
                        {
                            await DeleteParameterAsync(id, command[1]);
                        }
                        else
                        {
                            Console.WriteLine("You don't have permissions for the action");
                        }
                        break;

                    case "Return":
                        return;

                    default:
                        Console.WriteLine("Invalid Command");
                        break;
                }
            }
        }

        public async Task<bool> ShowControllerAsync(int id)
        {
            var response = await client.GetAsync($"Controller/{id}");

            if (response.IsSuccessStatusCode)
            {
                Controller? content = await response.Content.ReadFromJsonAsync<Controller>();
                if (content != null)
                {
                    content.Print();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Controller with that id doesn't exist");
                    return false;
                }
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return false;
            }

            return true;
        }

        public async Task UpdateParameterAsync(int id, string name, string value)
        {
            var response = await client.PatchAsync($"Controller/UpdateParameter/{id}?name={name}&value={value}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter updated");
            }
            else 
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
            }
        }

        public async Task RenameParameterAsync(int id, string name, string newName)
        {
            var response = await client.PatchAsync($"Controller/RenameParameter/{id}?name={name}&newName={newName}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter renamed");
                await ShowControllerAsync(id);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
            }
        }

        public async Task AddParameterAsync(int id, string name, int parameterId, double value, double minValue, double maxValue, bool IsConstant, bool IsInt)
        {
            string parameter = JsonSerializer.Serialize(new
            {
                id = parameterId,
                value,
                minValue,
                maxValue,
                IsConstant,
                IsInt
            });

            var httpContent = new StringContent(parameter, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"Controller/AddParameter/{id}?name={name}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Parameter added");
                await ShowControllerAsync(id);
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
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
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
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

        public async Task AddControllerAsync()
        {
            var response = await client.PostAsync($"Controller/AddController", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Controller added");
            }
            else
            {
                Console.WriteLine("Unknown error");
            }
        }

        public async Task DeleteControllerAsync(int id)
        {
            var response = await client.DeleteAsync($"Controller/{id}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Controller deleted");
            }
            else
            {
                Console.WriteLine("Invalid id");
            }
        }
    }
}
