namespace ConsoleManagementApp
{
    public class Controller
    {
        public int Id { get; set; }

        public string Parameters { get; set; } = "{ }";

        public void Print()
        {
            Console.WriteLine($"{Id}");
            Console.WriteLine($"{Parameters}");
        }
    }
}
