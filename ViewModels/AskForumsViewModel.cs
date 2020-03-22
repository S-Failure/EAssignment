using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class AskForumsViewModel
    {
        public int ForumId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Subject Name")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Division Name")]
        public int DivisionId { get; set; }

        [Required]
        [Display(Name = "Forum")]
        public string ForumData { get; set; }

        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }
    }
}
