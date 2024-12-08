using ControllerManagement.Data;

namespace ControllerManagement.Service
{
    public interface IControllerService
    {
        List<string> ShowAllControllers();

        void UpdateParameter(int id, string name, object value);

        void ReplaceParameter(int id, string name, string newName);

        void AddParameter(int id, string name, object value);

        void DeleteParameter(int id, string name);

        int AddController();

        void DeleteController(int id);

        Controller GetController(int id);
    }
}
