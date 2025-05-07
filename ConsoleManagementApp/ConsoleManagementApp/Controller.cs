using ControllerManagementApp;
using System.Text.Json;

namespace ConsoleManagementApp
{
    public class Controller
    {
        public int Id { get; set; }

        public string Parameters { get; set; } = "{ }";

        public void Print()
        {
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(Parameters);
            Console.WriteLine($"Controller {Id}");
            if (parameterDictionary == null)
            {
                return;
            }

            Console.WriteLine();

            foreach (KeyValuePair<string, Parameter> parameters in parameterDictionary)
            {
                Console.WriteLine(parameters.Key);
                Console.WriteLine(parameters.Value);
            }
        }
    }
}
