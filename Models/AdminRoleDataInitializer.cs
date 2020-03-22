using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public static class AdminRoleDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("utkarshpatil191199@gmail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "utkarshpatil191199@gmail.com";
                user.NormalizedUserName = "utkarshpatil191199@gmail.com";
                user.Email = "utkarshpatil191199@gmail.com";
                user.NormalizedEmail = "utkarshpatil191199@gmail.com";
                user.EmailConfirmed = true;
                user.PhoneNumber = "9870059553";
                user.PhoneNumberConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Super Admin").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Super Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Super Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
