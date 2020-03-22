using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("utkarshpatil191199@gmail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "utkarshpatil191199@gmail.com";
                user.Email = "utkarshpatil191199@gmail.com";
                //user.NormalizedEmail = "utkarshpatil191199@gmail.com";
                //user.NormalizedUserName = "utkarshpatil191199@gmail.com";
                user.PhoneNumber = "9870059553";
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = userManager.CreateAsync
                (user, "Utkarsh@1234").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                                        "Admin").Wait();
                    userManager.AddToRoleAsync(user,
                                        "Super Admin").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
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


    public static class UserAndRoleDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("johndoe@localhost").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "johndoe@localhost";
                user.Email = "johndoe@localhost";
                user.FirstName = "John";
                user.LastName = "Doe";

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                }
            }


            if (userManager.FindByEmailAsync("utkarshpatil191199@gmail").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "utkarshpatil191199@gmail";
                user.Email = "utkarshpatil191199@gmail";
                user.FirstName = "Utkarsh";
                user.LastName = "Patil";

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
