using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControllerManagement.Data
{
    public class ControllerManagementContext(DbContextOptions<ControllerManagementContext> dbContext) : IdentityDbContext<IdentityUser>(dbContext)
    {
        public DbSet<Controller> Controllers { get; set; }
    }
}
