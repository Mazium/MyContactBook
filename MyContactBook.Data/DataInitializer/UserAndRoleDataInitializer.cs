using Microsoft.AspNetCore.Identity;
using MyContactBook.Data.Context;
using MyContactBook.Model.Entities;


namespace MyContactBook.Data.DataInitializer
{
    public class UserAndRoleDataInitializer
    {
        public static async Task SeedData(ContactBookDbContext _dbcontext, UserManager<AppUser> userManager,
               RoleManager<IdentityRole> roleManager)
        {
            _dbcontext.Database.EnsureCreated();
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            string regularEmail = "nwaiwuchinedu@gmail.com";
            string regPassword = "Password@1";

            if (userManager.FindByEmailAsync(regularEmail).Result == null)
            {
                AppUser user = new()
                {
                    FirstName = "Nwaiwu",
                    LastName = "Chinedu",
                    //Gender = Gender.Male,
                    Email = "nwaiwuchinedu@gmail.com",
                    ImageUrl = "openForCorrection",                                      
                    UserName = "nwaiwuchinedu@gmail.com",
                    PhoneNumber = "+2348160593167",
                   
                };
                IdentityResult result = userManager.CreateAsync(user, regPassword).Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            string AdminEmail = "Ebitefrances@gmail.com";
            string AdminPassword = "Password@2";
            if (userManager.FindByEmailAsync(AdminEmail).Result == null)
            {
               
                AppUser user = new()
                {
                    FirstName = "Frances",
                    LastName = "Ebite",
                    //Gender = Gender.Female,
                    Email = "Ebitefrances@gmail.com",
                    UserName = "Ebitefrances@gmail.com",
                    ImageUrl = "openForCorrection",                    
                    PhoneNumber = "+2349018015592",
                    
                };
                IdentityResult result = userManager.CreateAsync(user, AdminPassword).Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Admin").Result == false)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}

