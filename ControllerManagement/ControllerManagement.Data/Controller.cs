
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControllerManagement.Data
{
    public class Controller
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Parameters { get; set; } = "{ }";
    }
}
