using ControllerManagement.Data;
using System.Reflection.Metadata;
using System.Text.Json;


namespace ControllerManagement.Service
{
    public class ControllerService : IControllerService
    {
        public ControllerService(ControllerManagementContext database)
        {
            this.database = database;
            ids = database.Controllers.Select(c => c.Id).ToHashSet();
        }

        private readonly ControllerManagementContext database;
        private readonly HashSet<int> ids;

        public int AddController()
        {
            int id = 1;
            while (ids.Contains(id))
            {
                id++;
            }
            Controller controller = new Controller();
            controller.Id = id;
            database.Controllers.Add(controller);
            database.SaveChanges();
            ids.Add(id);
            return controller.Id;
        }

        public void DeleteController(int id)
        {
            Controller controller = GetController(id);
            database.Controllers.Remove(controller);
            database.SaveChanges();
            ids.Remove(id);
        }

        public void AddParameter(int id, string name, Parameter parameter)
        {
            if (parameter.Value < parameter.MinValue || parameter.Value > parameter.MaxValue)
            {
                throw new ArgumentException("Out of range.");
            }

            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(controller.Parameters) ?? new Dictionary<string, Parameter>();
            if (parameterDictionary.Values.Any(p => p.Id == parameter.Id))
            {
                throw new ArgumentException("The parameter id already exists");
            }

            parameterDictionary[name] = parameter;
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }


        public void DeleteParameter(int id, string name)
        {
            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(controller.Parameters);
            if (parameterDictionary == null || !parameterDictionary.ContainsKey(name)) 
            {
                throw new ArgumentException("Parameter not found");
            }

            if (parameterDictionary[name].IsConstant)
            {
                throw new InvalidOperationException("You can't delete a constant parameter");
            }

            parameterDictionary.Remove(name);
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }

        public void RenameParameter(int id, string name, string newName)
        {
            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(controller.Parameters);
            if (parameterDictionary == null || !parameterDictionary.ContainsKey(name))
            {
                throw new ArgumentException("Parameter not found");
            }

            if (parameterDictionary[name].IsConstant)
            {
                throw new InvalidOperationException("You can't replace a constant parameter");
            }

            Parameter value = parameterDictionary[name];
            parameterDictionary.Remove(name);
            parameterDictionary.Add(newName, value);
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }

        public void UpdateParameter(int id, string name, double value)
        {

            Controller controller = GetController(id);
            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(controller.Parameters);
            if (parameterDictionary == null || !parameterDictionary.ContainsKey(name))
            {
                throw new ArgumentException("Parameter not found");
            }

            if (value < parameterDictionary[name].MinValue || value > parameterDictionary[name].MaxValue)
            {
                throw new ArgumentException("Out of range.");
            }

            if (parameterDictionary[name].IsConstant)
            {
                throw new InvalidOperationException("You can't update a constant parameter");
            }

            parameterDictionary[name].Value = value;
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }

        public List<string> ShowAllControllers()
        {
            return database.Controllers.Select(c => $"Controller {c.Id}").ToList();
        }


        public Controller GetController(int id) => database.Controllers.Find(id) ?? throw new ArgumentException("Invalid controller id");

        public void LoadParameter(int id, int parameterId, double value)
        {
            Controller controller = GetController(id);

            var parameterDictionary = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(controller.Parameters) 
                ?? throw new ArgumentException("Parameter not found");
            var parameter = parameterDictionary.Values.Single(p => p.Id == parameterId);

            if (value < parameter.MinValue || value > parameter.MaxValue)
            {
                throw new ArgumentException("Out of range.");
            }

            if (parameter.IsConstant)
            {
                throw new InvalidOperationException("You can't update a constant parameter");
            }

            parameter.Value = value;
            controller.Parameters = JsonSerializer.Serialize(parameterDictionary);
            database.SaveChanges();
        }
    }
}
