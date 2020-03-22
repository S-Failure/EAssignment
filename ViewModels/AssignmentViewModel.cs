using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class AssignmentViewModel
    {
        public int AssignmentId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int DivisionId { get; set; }
        public string AssignmentName { get; set; }

        [Required]
        [Display(Name = "Upload File")]
        public IFormFile Path { get; set; }

        public string UserId { get; set; }
    }
}
