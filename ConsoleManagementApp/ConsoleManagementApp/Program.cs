﻿using ConsoleManagementApp;

ControllerMenu controllerMenu = new ControllerMenu();

while (true)
{
    Console.WriteLine("Controller Management App");
    Console.WriteLine("Available commands:");
    Console.WriteLine("Login");
    Console.WriteLine("Logout");
    Console.WriteLine("ShowControllers");
    if (controllerMenu.IsAdmin())
    {
        Console.WriteLine("ShowUsers");
        Console.WriteLine("AddUser {username} {password} {role}");
        Console.WriteLine("DeleteUser {username}");
        Console.WriteLine("AddController");
        Console.WriteLine("DeleteController {id}");
    }
    Console.WriteLine("Edit {id}");
    Console.WriteLine("Exit");
    Console.WriteLine();

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

        case "ShowUsers":
            if (!controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }
            await controllerMenu.ShowUsersAsync();
            break;

        case "AddUser":
            if (!controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }

            if (command.Length != 4)
            {
                Console.WriteLine("Invalid arguments");
                break;
            }
            await controllerMenu.AddUserAsync(command[1], command[2], command[3]);
            break;

        case "DeleteUser":
            if (!controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }

            if (command.Length != 2)
            {
                Console.WriteLine("Invalid arguments");
                break;
            }
            await controllerMenu.DeleteUserAsync(command[1]);
            break;

        case "AddController":
            if (!controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }
            await controllerMenu.AddControllerAsync();
            break;

        case "DeleteController":
            if (!controllerMenu.IsAdmin())
            {
                Console.WriteLine("You don't have permissions for that action");
                break;
            }

            int controllerId;

            if (command.Length < 2 || !int.TryParse(command[1], out controllerId))
            {
                Console.WriteLine("Invalid controller id");
                break;
            }
            await controllerMenu.DeleteControllerAsync(controllerId);
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
            await controllerMenu.EditAsync(id);
            break;

        case "Exit":
            return;

        default:
            Console.WriteLine("Invalid Command");
            break;
    }

    Console.WriteLine();
}


