using EAssignment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class CreateSubjectViewModel
    {
        public CreateSubjectViewModel()
        {
            //ClassId = new IEnumerable<int>();
        }

        public int SubjectId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public int ClassId { get; set; }
    }
}
