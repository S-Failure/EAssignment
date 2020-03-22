using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class FilesSettingViewModel
    {
        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Subject Name")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Division Name")]
        public int DivisionId { get; set; }
    }
}
