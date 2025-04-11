using ControllerManagement.Data;

namespace ControllerManagement.Service
{
    public interface IControllerService
    {
        List<string> ShowAllControllers();

        void UpdateParameter(int id, string name, double value);

        void RenameParameter(int id, string name, string newName);

        void AddParameter(int id, string name, Parameter value);

        void DeleteParameter(int id, string name);

        int AddController();

        void DeleteController(int id);

        Controller GetController(int id);
    }
}
