using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EAssignment.Models;
using EAssignment.Security;
using EAssignment.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace EAssignment.Controllers
{
    public class UDPController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<UDPController> logger;
        private readonly ICDSRepository _cdsRepository;
        private readonly IDataProtector protector;
        private readonly AppDbContext _context;

        public UDPController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                        ILogger<UDPController> logger, ICDSRepository cdsRepository,
                                        IDataProtectionProvider dataProtectionProvider,
                                DataProtectionPurposeStrings dataProtectionPurposeStrings, AppDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this._cdsRepository = cdsRepository;
            protector = dataProtectionProvider
                .CreateProtector(dataProtectionPurposeStrings.EnquiryIdRouteValue);
            this._context = context;
        }

        private void ListOfClasses()
        {
            List<Class> classesList = new List<Class>();
            classesList = _context.Classes.ToList();
            classesList.Insert(0, new Class { ClassId = 0, ClassName = "Select Class" });
            ViewBag.ListOfClasses = classesList;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                DOB = user.DOB,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(EditUserViewModel model)
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.PhoneNumber = model.Phone;
                user.DOB = model.DOB;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ListClasses()
        {
            var model = _cdsRepository.GetAllClass()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.ClassId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpGet]
        public IActionResult ListDivisions()
        {
            var model = _cdsRepository.GetAllDivision()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.DivisionId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpGet]
        public IActionResult ListSubjects()
        {
            var model = _cdsRepository.GetAllSubject()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.SubjectId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateClass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateClass(Class model)
        {
            if (ModelState.IsValid)
            {
                Class newClass = new Class
                {
                    ClassName = model.ClassName
                };

                _cdsRepository.AddClass(newClass);
                return RedirectToAction("ListClasses", "UDP");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateDivision()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDivision(Division model)
        {
            if (ModelState.IsValid)
            {
                Division newDivision = new Division
                {
                    DivisionName = model.DivisionName
                };

                _cdsRepository.AddDivision(newDivision);
                return RedirectToAction("ListDivisions", "UDP");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateSubject()
        {
            //classesList = (from classes in _context.Classes
            //               select classes).ToList();

            ListOfClasses();
            return View();
        }

        [HttpPost]
        public IActionResult CreateSubject(CreateSubjectViewModel model)
        {
            if (model.ClassId == 0)
            {
                ModelState.AddModelError("", "The select class name field required.");
            }

            if (ModelState.IsValid)
            {
                Subject newSubject = new Subject
                {
                    SubjectName = model.SubjectName,
                    ClassId = model.ClassId
                };

                var result = _cdsRepository.AddSubject(newSubject);

                if (result == null)
                {
                    ModelState.AddModelError("", "Please select Class Name.");
                    return View(model);
                }

                return RedirectToAction("ListSubjects", "UDP");
            }

            ListOfClasses();

            return View(model);
        }

        [HttpGet]
        public IActionResult EditClass(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));

            Class classes = _cdsRepository.GetClass(decriptedId);

            Class newClass = new Class()
            {
                ClassId = classes.ClassId,
                ClassName = classes.ClassName
            };

            return View(newClass);
        }

        [HttpPost]
        public IActionResult EditClass(Class model)
        {
            if (ModelState.IsValid)
            {
                Class classes = _cdsRepository.GetClass(model.ClassId);

                classes.ClassName = model.ClassName;

                _cdsRepository.UpdateClass(classes);
                return RedirectToAction("ListClasses");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditDivision(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));

            Division division = _cdsRepository.GetDivision(decriptedId);

            Division newDivision = new Division()
            {
                DivisionId = division.DivisionId,
                DivisionName = division.DivisionName
            };

            return View(newDivision);
        }

        [HttpPost]
        public IActionResult EditDivision(Division model)
        {
            if (ModelState.IsValid)
            {
                Division division = _cdsRepository.GetDivision(model.DivisionId);

                division.DivisionName = model.DivisionName;

                _cdsRepository.UpdateDivision(division);
                return RedirectToAction("ListDivisions");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditSubject(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));

            Subject subject = _cdsRepository.GetSubject(decriptedId);

            Subject newSubject = new Subject()
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                ClassId = subject.ClassId
            };

            ListOfClasses();

            return View(newSubject);
        }

        [HttpPost]
        public IActionResult EditSubject(Subject model)
        {
            if(model.ClassId == 0)
            {
                ModelState.AddModelError("", "The subject name field required.");
            }

            if (ModelState.IsValid)
            {
                Subject subject = _cdsRepository.GetSubject(model.SubjectId);

                subject.SubjectName = model.SubjectName;
                subject.ClassId = model.ClassId;

                _cdsRepository.UpdateSubject(subject);
                return RedirectToAction("ListSubjects");
            }

            ListOfClasses();

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteClass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteClass(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            _cdsRepository.DeleteClass(decriptedId);
            return RedirectToAction("ListClasses");
        }

        [HttpGet]
        public IActionResult DeleteDivision()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteDivision(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            _cdsRepository.DeleteDivision(decriptedId);
            return RedirectToAction("ListDivisions");
        }

        [HttpGet]
        public IActionResult DeleteSubject()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteSubject(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            _cdsRepository.DeleteSubject(decriptedId);
            return RedirectToAction("ListSubjects");
        }

        [HttpGet]
        public IActionResult Forums()
        {
            var model = _cdsRepository.GetAllForum()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.ForumId.ToString());
                                return e;
                            })
                            .OrderByDescending(e => e.ForumId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Forums(Forum model)
        {
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AskForums()
        {
            var id = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new AskForumsViewModel
            {
                UserId = user.Id,
                ClassId = user.ClassId,
                SubjectId = user.SubjectId,
                DivisionId = user.DivisionId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AskForums(AskForumsViewModel model)
        {
            if(model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check your Files Setting first or Update.");
            }

            if(ModelState.IsValid)
            {
                Forum newForum = new Forum
                {
                    ClassId = model.ClassId,
                    SubjectId = model.SubjectId,
                    DivisionId = model.DivisionId,
                    UserId = model.UserId,
                    ForumData = model.ForumData
                };

                _cdsRepository.AddForum(newForum);
                return RedirectToAction("Forums", "UDP");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ReplyForums(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            Forum forum = _cdsRepository.GetForum(decriptedId);

            ReplyForumsViewModel newForum = new ReplyForumsViewModel()
            {
                ForumId = forum.ForumId,
                ClassId = forum.ClassId,
                SubjectId = forum.SubjectId,
                DivisionId = forum.DivisionId,
                ForumData = forum.ForumData,
                UserId = forum.UserId
            };
            return View(newForum);
        }

        [HttpPost]
        public IActionResult ReplyForums(ReplyForumsViewModel model)
        {
            if (model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check your Files Setting first or Update.");
            }

            if (ModelState.IsValid)
            {
                Forum forum = _cdsRepository.GetForum(model.ForumId);

                forum.ClassId = model.ClassId;
                forum.SubjectId = model.SubjectId;
                forum.DivisionId = model.DivisionId;
                forum.ForumData = model.ForumData;
                forum.UserId = model.UserId;

                _cdsRepository.UpdateForum(forum);
                return RedirectToAction("Forums", "UDP");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteForums()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteForums(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            _cdsRepository.DeleteForum(decriptedId);
            return RedirectToAction("Forums");
        }
    }
}