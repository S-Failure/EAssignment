using EAssignment.Models;
using EAssignment.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class RegisterAdminViewModel
    {
        public RegisterAdminViewModel()
        {
            Claims = new List<UserClaim>();
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Admin Email")]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [ValidEmailDomain(allowedDomain: "gmail.com",
            ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
