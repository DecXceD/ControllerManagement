using ControllerManagement.Data;
using System.Text.Json;


namespace ControllerManagement.Service
{
    public class ControllerService : IControllerService
    {
        public ControllerService(ControllerManagementContext database)
        {
            this.database = database;
        }

        private ControllerManagementContext database;

        public void AddController()
        {
            Controller controller = new Controller();
            database.Controllers.Add(controller);
            database.SaveChanges();
        }

        public void AddParameter(int id, string name, object value)
        {
            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(controller.Parameters) ?? new Dictionary<string, object>();
            parameterDictionary[name] = value;
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }

        public void DeleteController(int id)
        {
            Controller controller = GetController(id);
            database.Controllers.Remove(controller);
            database.SaveChanges();
        }

        public void DeleteParameter(int id, string name)
        {
            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(controller.Parameters);
            if (parameterDictionary == null || !parameterDictionary.ContainsKey(name)) 
            {
                throw new ArgumentException("Parameter not found");
            }

            parameterDictionary.Remove(name);
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }

        public void ReplaceParameter(int id, string name, string newName)
        {
            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(controller.Parameters);
            if (parameterDictionary == null || !parameterDictionary.ContainsKey(name))
            {
                throw new ArgumentException("Parameter not found");
            }

            object value = parameterDictionary[name];
            parameterDictionary.Remove(name);
            parameterDictionary.Add(newName, value);
            controller.Parameters = JsonSerializer.Serialize(value);
            database.SaveChanges();
        }

        public List<string> ShowAllControllers()
        {
            return database.Controllers.Select(c => $"Controller {c.Id}").ToList();
        }

        public void UpdateParameter(int id, string name, object value)
        {
            AddParameter(id, name, value);
        }

        public Controller GetController(int id) => database.Controllers.Find(id) ?? throw new ArgumentException("Invalid controller id");
    }
}
