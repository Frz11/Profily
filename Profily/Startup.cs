using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Profily.Models;

[assembly: OwinStartupAttribute(typeof(Profily.Startup))]
namespace Profily
{
    public partial class Startup
    {
        ApplicationDbContext context = ApplicationDbContext.Create();

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminUserAndApplicationRoles();


        }
        public void CreateAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Administrator"))
            {
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                user.LastName = "Master";
                user.FirstName = "Admin";
                var adminCreated = userManager.Create(user, "Admin1!");
                if (adminCreated.Succeeded)
                {
                    Profile profile = new Profile { UserId = user.Id, Description = "My Profile!" };
                    try
                    {
                        context.Profiles.Add(profile);
                        context.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                    Album album = new Album { Name = "Main", Description = "", IsDefault = true, ProfileId = user.Id };
                    try
                    {
                        context.Albums.Add(album);
                        context.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                    userManager.AddToRole(user.Id, "Administrator");
                    
                }

            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
    }
}
