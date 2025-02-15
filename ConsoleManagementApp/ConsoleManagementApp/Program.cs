using ConsoleManagementApp;

ControllerMenu controllerMenu = new ControllerMenu();

while (true)
{
    Console.WriteLine("Controller Management App");
    Console.WriteLine("Available commands:");
    Console.WriteLine("Login");
    Console.WriteLine("Logout");
    Console.WriteLine("ShowControllers");
    Console.WriteLine("Edit");
    Console.WriteLine("Exit");

    string[] command = Console.ReadLine().Split();
    switch (command[0])
    {
        case "Login":
            await controllerMenu.LoginAsync();
            break;

        case "Logout":
            controllerMenu.Logout();
            break;

        case "ShowControllers":
            await controllerMenu.ShowControllersAsync();
            break;

        case "Edit":
            if (!controllerMenu.IsWorker() && !controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }

            int id;

            if (command.Length < 2 || !int.TryParse(command[1], out id))
            {
                Console.WriteLine("Invalid controller id");
                break;
            }
            await controllerMenu.Edit(id);
            break;

        case "Exit":
            return;

        default:
            Console.WriteLine("Invalid Command");
            break;
    }
}


