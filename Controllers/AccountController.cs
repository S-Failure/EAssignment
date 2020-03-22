using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EAssignment.Models;
using EAssignment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EAssignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger, IWebHostEnvironment hostingEnvironment, IConfiguration configuration,
            RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            this._configuration = configuration;
            this.roleManager = roleManager;
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("AddPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (!(_context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "Database Can't Found";
                return View("DatabaseError");
            }

            if (userManager.Users.Count() == 0 && roleManager.Roles.Count() == 0)
            {
                return RedirectToAction("RegisterAdmin", "Account");
            }

            bool isAuthenticated = User.Identity.IsAuthenticated;

            if(isAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    ViewBag.Validate = $"Done";
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                        model.RememberMe, true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                ViewBag.Validate = $"Done";
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Error from external provider: {remoteError}");
                ViewBag.Validate = $"Done";

                return View("Login", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Error loading external login information.");
                ViewBag.Validate = $"Done";

                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                user = await userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    ViewBag.Validate = $"Done";
                    return View("Login", loginViewModel);
                }
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(
                                        info.LoginProvider, info.ProviderKey,
                                        isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);

                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                        new { userId = user.Id, token = token }, Request.Scheme);

                        var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress("codewithprogrammer@gmail.com", "EAssignment");
                        List<EmailAddress> tos = new List<EmailAddress>
                        {
                            new EmailAddress(user.Email, user.FirstName + user.LastName)
                        };

                        var subject = "Account Confirmation";
                        var htmlContent = "<a class='btn btn-success' href='" + confirmationLink + "'>Confirm Account</a>";
                        //var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
                        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, false);
                        var response = await client.SendEmailAsync(msg);

                        logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                            "email, by clicking on the confirmation link we have emailed you";
                        return View("Error");
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on codewithprogrammer@gmail.com";

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("codewithprogrammer@gmail.com", "EAssignment");
                    List<EmailAddress> tos = new List<EmailAddress>
                    {
                        new EmailAddress(model.Email, model.Email)
                    };

                    var subject = "Reset Password";
                    var htmlContent = "<a class='btn btn-success' href='" + passwordResetLink + "'>Reset Password</a>";
                    //var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, false);
                    var response = await client.SendEmailAsync(msg);

                    return View("ForgotPasswordConfirmation");
                }

                ModelState.AddModelError("", "Email Not Found");

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (!(_context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "Database Can't Found";
                return View("DatabaseError");
            }

            if (userManager.Users.Count() == 0 && roleManager.Roles.Count() == 0)
            {
                return RedirectToAction("RegisterAdmin", "Account");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                    DOB = model.DOB,
                    //PhotoPath = uniqueFileName
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Student").Wait();
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = user.Id, token = token }, Request.Scheme);

                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("codewithprogrammer@gmail.com", "EAssignment");
                    List<EmailAddress> tos = new List<EmailAddress>
                    {
                        new EmailAddress(model.Email, model.FirstName + model.LastName)
                    };

                    var subject = "Account Confirmation";
                    var htmlContent = "<a class='btn btn-success' href='" + confirmationLink + "'>Confirm Account</a>";
                    //var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, false);
                    var response = await client.SendEmailAsync(msg);

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                        "email, by clicking on the confirmation link we have emailed you";

                    //return RedirectToAction("Login", "Account");
                    return View("Error");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterAdmin()
        {
            if (!(_context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "Database Can't Found";
                return View("DatabaseError");
            }

            if (userManager.Users.Count() != 0 && roleManager.Roles.Count() != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            RegisterAdminViewModel model = new RegisterAdminViewModel();

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                userClaim.IsSelected = true;
                model.Claims.Add(userClaim);
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = model.IsEmailConfirmed
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    IdentityRole superAdminRole = new IdentityRole
                    {
                        Name = "Super Admin"
                    };

                    IdentityResult superAdminResult = await roleManager.CreateAsync(superAdminRole);

                    if (superAdminResult.Succeeded)
                    {
                        IdentityRole adminRole = new IdentityRole
                        {
                            Name = "Admin"
                        };
                        IdentityRole facultyRole = new IdentityRole
                        {
                            Name = "Faculty"
                        };
                        IdentityRole studentRole = new IdentityRole
                        {
                            Name = "Student"
                        };

                        IdentityResult facultyResult = await roleManager.CreateAsync(facultyRole);
                        IdentityResult studentResult = await roleManager.CreateAsync(studentRole);

                        IdentityResult adminResult = await roleManager.CreateAsync(adminRole);

                        if (!facultyResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Cannot create Facuty Role");
                            return View(model);
                        }
                        if (!studentResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Cannot create Student Role");
                            return View(model);
                        }

                        if (adminResult.Succeeded)
                        {
                            result = await userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

                            if (!result.Succeeded)
                            {
                                ModelState.AddModelError("", "Cannot add selected claims to user");
                                return View(model);
                            }

                            if (result.Succeeded)
                            {
                                userManager.AddToRoleAsync(user, "Super Admin").Wait();
                                userManager.AddToRoleAsync(user, "Admin").Wait();
                                ViewBag.ErrorTitle = "Registration successful";
                                ViewBag.ErrorMessage = "You can login now";

                                return View("Error");
                            }

                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                        }

                        foreach (IdentityError error in adminResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    foreach (IdentityError error in superAdminResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}