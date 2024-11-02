
using System.ComponentModel.DataAnnotations;

namespace ControllerManagement.Data
{
    public class Controller
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Parameters { get; set; } = string.Empty;
    }
}
