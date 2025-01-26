Console.WriteLine("Controller Management App");
Console.WriteLine("Available commands:");
Console.WriteLine("Login");
Console.WriteLine("Logout");
Console.WriteLine("Show Controllers");
Console.WriteLine("Exit");
string command = Console.ReadLine();
switch(command){
    case "Login":
        await LoginAsync();
        break;

    case "Logout":
        Logout();
        break;

    case "Show Controllers":
        ShowControllers();
        break;

    case "Exit":
        return;

    default:
        Console.WriteLine("Invalid Command");
        break;
}
static async Task LoginAsync()
{
    Console.WriteLine("Enter Username:");
    string username = Console.ReadLine();
    Console.WriteLine("Enter Password:");
    string password = Console.ReadLine();
    HttpClient client = new HttpClient();

    var response = await client.PostAsync($"https://localhost:44340/api/User/Login?username={username}&password={password}", null);
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}

static void Logout()
{

}

static void ShowControllers()
{

}
