using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EAssignment.Models;
using EAssignment.Security;
using EAssignment.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace EAssignment.Controllers
{
    public class FilesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<FilesController> logger;
        private readonly IFilesRepository _filesRepository;
        private readonly IDataProtector protector;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public FilesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                        ILogger<FilesController> logger, IFilesRepository filesRepository,
                                        IDataProtectionProvider dataProtectionProvider,
                                DataProtectionPurposeStrings dataProtectionPurposeStrings, AppDbContext context,
                                IWebHostEnvironment hostingEnvironment)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this._filesRepository = filesRepository;
            protector = dataProtectionProvider
                .CreateProtector(dataProtectionPurposeStrings.EnquiryIdRouteValue);
            this._context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadSubmit(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            Submit submit = _filesRepository.GetSubmit(decriptedId);

            if (submit == null)
            {
                ViewBag.ErrorMessage = $"file name cannot be found";
                return View("NotFound");
            }

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/Submits", submit.SubmitPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(submit.SubmitName));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadAssignment(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            Assignment assignment = _filesRepository.GetAssignment(decriptedId);

            if (assignment == null)
            {
                ViewBag.ErrorMessage = $"file name cannot be found";
                return View("NotFound");
            }

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/Assignments", assignment.AssignmentPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(assignment.AssignmentName));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        private void ListOfClasses()
        {
            List<Class> classesList = new List<Class>();
            classesList = _context.Classes.ToList();
            classesList.Insert(0, new Class { ClassId = 0, ClassName = "Select Class" });
            ViewBag.ListOfClasses = classesList;
        }

        private void ListOfSubjects()
        {
            List<Subject> subjectsList = new List<Subject>();
            subjectsList = _context.Subjects.ToList();
            subjectsList.Insert(0, new Subject { SubjectId = 0, SubjectName = "Select Subject" });
            ViewBag.ListOfSubjects = subjectsList;
        }

        private void ListOfDivisions()
        {
            List<Division> divisionList = new List<Division>();
            divisionList = _context.Divisions.ToList();
            divisionList.Insert(0, new Division { DivisionId = 0, DivisionName = "Select Division" });
            ViewBag.ListOfDivisions = divisionList;
        }

        private JsonResult ListOfSubjectsByClassId(int ClassId)
        {
            List<Subject> subjectList = new List<Subject>();
            subjectList = (from subject in _context.Subjects
                           where subject.ClassId == ClassId
                           select subject).ToList();
            subjectList.Insert(0, new Subject { SubjectId = 0, SubjectName = "Select Subject" });
            return Json(new SelectList(subjectList, "SubjectId", "SubjectName"));
        }

        [HttpGet]
        public async Task<IActionResult> Submit()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new SubmitViewModel()
            {
                ClassId = user.ClassId,
                SubjectId = user.SubjectId,
                DivisionId = user.DivisionId,
                UserId = user.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(SubmitViewModel model)
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            if (model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check the file settings.");
            }

            string uniqueFileName = ProcessUploadedFile(model);
            string filename = getFileName(model);

            if (ModelState.IsValid)
            {
                FileSupportExtension(model);

                if (ModelState.IsValid)
                {
                    Submit submit = new Submit()
                    {
                        ClassId = model.ClassId,
                        SubjectId = model.SubjectId,
                        DivisionId = model.DivisionId,
                        UserId = model.UserId,
                        SubmitPath = uniqueFileName,
                        SubmitName = filename
                    };
                    _filesRepository.AddSubmit(submit);

                    return RedirectToAction("Submitted", "Files");
                }

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new AssignmentViewModel()
            {
                ClassId = user.ClassId,
                SubjectId = user.SubjectId,
                DivisionId = user.DivisionId,
                UserId = user.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(AssignmentViewModel model)
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            if (model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check the file settings.");
            }

            string uniqueFileName = ProcessUploadedFileAssignment(model);
            string filename = getFileNameAssignment(model);

            if (ModelState.IsValid)
            {
                FileSupportExtensionAssignment(model);

                if (ModelState.IsValid)
                {
                    Assignment assignment = new Assignment()
                    {
                        ClassId = model.ClassId,
                        SubjectId = model.SubjectId,
                        DivisionId = model.DivisionId,
                        UserId = model.UserId,
                        AssignmentPath = uniqueFileName,
                        AssignmentName = filename
                    };
                    _filesRepository.AddAssignment(assignment);

                    return RedirectToAction("Uploaded", "Files");
                }

                return View(model);
            }

            return View(model);
        }

        private void FileSupportExtension(SubmitViewModel model)
        {
            var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx", "java" };
            var fileExt = Path.GetExtension(model.Path.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                ModelState.AddModelError("", "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT/JAVA File");
            }
        }

        private string ProcessUploadedFile(SubmitViewModel model)
        {
            string uniqueFileName = null;
            if (model.Path != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Submits");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ClassId + "_" + model.SubjectId + "_" + model.DivisionId + "_" + model.Path.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Path.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        private string getFileName(SubmitViewModel model)
        {
            string fileName = null;
            if (model.Path != null)
            {
                fileName = model.Path.FileName;
            }

            return fileName;
        }

        private void FileSupportExtensionAssignment(AssignmentViewModel model)
        {
            var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx", "java" };
            var fileExt = Path.GetExtension(model.Path.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                ModelState.AddModelError("", "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT/JAVA File");
            }
        }

        private string ProcessUploadedFileAssignment(AssignmentViewModel model)
        {
            string uniqueFileName = null;
            if (model.Path != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Assignments");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ClassId + "_" + model.SubjectId + "_" + model.DivisionId + "_" + model.Path.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Path.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        private string getFileNameAssignment(AssignmentViewModel model)
        {
            string fileName = null;
            if (model.Path != null)
            {
                fileName = model.Path.FileName;
            }

            return fileName;
        }

        [HttpGet]
        public async Task<IActionResult> Submitted()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = _filesRepository.GetAllSubmit()
                            .Where(e=> e.ClassId == user.ClassId && e.SubjectId == user.SubjectId && e.DivisionId == user.DivisionId && e.UserId == user.Id)
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.SubmitId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Uploaded()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = _filesRepository.GetAllAssignment()
                            .Where(e => e.ClassId == user.ClassId && e.SubjectId == user.SubjectId && e.DivisionId == user.DivisionId && e.UserId == user.Id)
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.AssignmentId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpGet]
        public IActionResult EditSubmit(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            var submit = _filesRepository.GetSubmit(decriptedId);

            if (submit == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditSubmitViewModel()
            {
                SubmitId = submit.SubmitId,
                ClassId = submit.ClassId,
                SubjectId = submit.SubjectId,
                DivisionId = submit.DivisionId,
                UserId = submit.UserId,
                ExistingPath = submit.SubmitPath,
                SubmitName = submit.SubmitName,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditSubmit(EditSubmitViewModel model)
        {
            Submit submit = _filesRepository.GetSubmit(model.SubmitId);

            if (model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check the file settings.");
            }

            if (ModelState.IsValid)
            {
                submit.ClassId = model.ClassId;
                submit.SubjectId = model.SubjectId;
                submit.DivisionId = model.DivisionId;
                submit.UserId = model.UserId;
                if (model.Path != null)
                {
                    if (model.ExistingPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "Submits", model.ExistingPath);
                        System.IO.File.Delete(filePath);
                    }
                    submit.SubmitPath = ProcessUploadedFile(model);
                    submit.SubmitName = getFileName(model);
                    FileSupportExtension(model);
                }
                if (ModelState.IsValid)
                {
                    _filesRepository.UpdateSubmit(submit);
                    return RedirectToAction("Submitted");
                }
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditUpload(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            var assignment = _filesRepository.GetAssignment(decriptedId);

            if (assignment == null)
            {
                ViewBag.ErrorMessage = $"File with id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditAssignmentViewModel()
            {
                AssignmentId = assignment.AssignmentId,
                ClassId = assignment.ClassId,
                SubjectId = assignment.SubjectId,
                DivisionId = assignment.DivisionId,
                UserId = assignment.UserId,
                ExistingPath = assignment.AssignmentPath,
                AssignmentName = assignment.AssignmentName,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditUpload(EditAssignmentViewModel model)
        {
            Assignment assignment = _filesRepository.GetAssignment(model.AssignmentId);

            if (model.ClassId == 0 && model.SubjectId == 0 && model.DivisionId == 0)
            {
                ModelState.AddModelError("", "Please check the file settings.");
            }

            if (ModelState.IsValid)
            {
                assignment.ClassId = model.ClassId;
                assignment.SubjectId = model.SubjectId;
                assignment.DivisionId = model.DivisionId;
                assignment.UserId = model.UserId;
                if (model.Path != null)
                {
                    if (model.ExistingPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "Assignments", model.ExistingPath);
                        System.IO.File.Delete(filePath);
                    }
                    assignment.AssignmentPath = ProcessUploadedFileAssignment(model);
                    assignment.AssignmentName = getFileNameAssignment(model);
                    FileSupportExtensionAssignment(model);
                }
                if (ModelState.IsValid)
                {
                    _filesRepository.UpdateAssignment(assignment);
                    return RedirectToAction("Uploaded");
                }
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteSubmits()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteSubmits(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            Submit submit = _filesRepository.GetSubmit(decriptedId);

            if (submit.SubmitPath != null)
            {
                string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                    "Submits", submit.SubmitPath);
                System.IO.File.Delete(filePath);
            }

            _filesRepository.DeleteSubmit(decriptedId);
            return RedirectToAction("Submitted");
        }

        [HttpGet]
        public IActionResult DeleteUpload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteUpload(string id)
        {
            var decriptedId = Convert.ToInt32(protector.Unprotect(id));
            Assignment assignment = _filesRepository.GetAssignment(decriptedId);

            if (assignment.AssignmentPath != null)
            {
                string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                    "Assignments", assignment.AssignmentPath);
                System.IO.File.Delete(filePath);
            }

            _filesRepository.DeleteAssignment(decriptedId);
            return RedirectToAction("Uploaded");
        }

        [HttpGet]
        public async Task<IActionResult> FilesSetting()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new FilesSettingViewModel
            {
                Id = user.Id,
                ClassId = user.ClassId,
                SubjectId = user.SubjectId,
                DivisionId = user.DivisionId
            };

            ListOfClasses();
            ListOfDivisions();
            ListOfSubjects();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FilesSetting(FilesSettingViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.Id} cannot be found";
                return View("NotFound");
            }

            if (model.ClassId == 0)
            {
                ModelState.AddModelError("", "The select class name field required.");
            }

            if (model.SubjectId == 0)
            {
                ModelState.AddModelError("", "The select subject name field required.");
            }

            if (model.DivisionId == 0)
            {
                ModelState.AddModelError("", "The select division name field required.");
            }

            if (ModelState.IsValid)
            {
                var SubjectId = HttpContext.Request.Form["SubjectId"].ToString();
                user.Id = model.Id;
                user.ClassId = model.ClassId;
                user.SubjectId = model.SubjectId;
                user.DivisionId = model.DivisionId;



                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ListOfClasses();
            ListOfDivisions();
            ListOfSubjects();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AllSubmitted()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = _filesRepository.GetAllSubmit()
                            .Where(e => e.ClassId == user.ClassId && e.DivisionId == user.DivisionId && e.SubjectId == user.SubjectId)
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.SubmitId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpPost]
        public IActionResult AllSubmitted(Submit model)
        {
            return View("AllSubmitted");
        }

        [HttpGet]
        public async Task<IActionResult> AllUploaded()
        {
            var id = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = _filesRepository.GetAllAssignment()
                            .Where(e => e.ClassId == user.ClassId && e.DivisionId == user.DivisionId && e.SubjectId == user.SubjectId)
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.AssignmentId.ToString());
                                return e;
                            });

            return View(model);
        }

        [HttpPost]
        public IActionResult AllUploaded(Assignment model)
        {
            return View("AllUploaded");
        }
    }
}