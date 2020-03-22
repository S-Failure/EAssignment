using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using EAssignment.Security;
using EAssignment.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;

namespace EAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEnquiryRepository _enquiryRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProtector protector;
        private readonly AppDbContext _context;

        public HomeController(IEnquiryRepository enquiryRepository,
                                IDataProtectionProvider dataProtectionProvider,
                                DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                ILogger<HomeController> logger, AppDbContext context,
                              RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            this._enquiryRepository = enquiryRepository;
            _logger = logger;
            protector = dataProtectionProvider
                .CreateProtector(dataProtectionPurposeStrings.EnquiryIdRouteValue);
            this._context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
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

        [AllowAnonymous]
        public IActionResult About()
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Contact()
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
        public IActionResult Contact(EnquiryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Enquiry newEnquiry = new Enquiry
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Subject = model.Subject,
                    Message = model.Message,
                    Reply = null
                };

                _enquiryRepository.Add(newEnquiry);
                return RedirectToAction("EnquirySuccess");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnquirySuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllEnquiry()
        {
            var model = _enquiryRepository.GetAllEnquiry()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.ID.ToString());
                                return e;
                            })
                            .OrderByDescending(e => e.ID);

            return View(model);
        }

        [HttpGet]
        public IActionResult ReplyEnquiry(string id)
        {
            var decryptedId = Convert.ToInt32(protector.Unprotect(id));

            Enquiry enquiry = _enquiryRepository.GetEnquiry(decryptedId);

            if (enquiry == null)
            {
                ViewBag.ErrorMessage = $"Enquiry with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            ReplyEnquiryViewModel newEnquiry = new ReplyEnquiryViewModel()
            {
                ID = enquiry.ID,
                Name = enquiry.Name,
                Email = enquiry.Email,
                Subject = enquiry.Subject,
                Reply = enquiry.Reply
            };

            return View(newEnquiry);
        }

        [HttpPost]
        public IActionResult ReplyEnquiry(ReplyEnquiryViewModel model)
        {
            if(ModelState.IsValid)
            {
                Enquiry enquiry = _enquiryRepository.GetEnquiry(model.ID);

                if (enquiry == null)
                {
                    ViewBag.ErrorMessage = $"Enquiry with Id = {model.ID} cannot be found";
                    return View("NotFound");
                }

                enquiry.ID = model.ID;
                enquiry.Name = model.Name;
                enquiry.Email = model.Email;
                enquiry.Subject = model.Subject;
                enquiry.Reply = model.Reply;

                _enquiryRepository.Update(enquiry);
                return RedirectToAction("AllEnquiry");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteEnquiry()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult DeleteEnquiry(string id)
        {
            var decryptedId = Convert.ToInt32(protector.Unprotect(id));

            if (decryptedId == 0)
            {
                ViewBag.ErrorMessage = $"Enquiry with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            _enquiryRepository.Delete(decryptedId);
            return RedirectToAction("AllEnquiry");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult ErrorDatabase()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewData["statusCode"] = HttpContext.Response.StatusCode;
            ViewData["message"] = exception.Error.Message;
            //ViewData["stackTrace"] = exception.Error.StackTrace;
            ViewData["stackTrace"] = exception.Error.StackTrace;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult HowToInstall()
        {
            if (!(_context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                return View();
            }
            return View("Index");
        }
    }
}
