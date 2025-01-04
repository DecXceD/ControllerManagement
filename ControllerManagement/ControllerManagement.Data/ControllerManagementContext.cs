using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControllerManagement.Data
{
    public class ControllerManagementContext(DbContextOptions<ControllerManagementContext> dbContext) : IdentityDbContext<IdentityUser>(dbContext)
    {
        public DbSet<Controller> Controllers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IdentityRole role = new IdentityRole
            {
                Name = Data.UserRoles.Admin,
                Id = "091287346556"
            };

            modelBuilder.Entity<IdentityRole>().HasData(role);
            IdentityUser user = new IdentityUser
            {
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Id = "0192837465564738190"
            };

            user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, "terces");
            modelBuilder.Entity<IdentityUser>().HasData(user);

            IdentityUserRole<string> userRole = new IdentityUserRole<string> { RoleId = role.Id, UserId = user.Id };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRole);

            base.OnModelCreating(modelBuilder);
        }
    }
}
