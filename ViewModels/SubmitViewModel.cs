using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class SubmitViewModel
    {
        public int SubmitId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int DivisionId { get; set; }
        public string SubmitName { get; set; }

        [Required]
        [Display(Name = "Submit File")]
        public IFormFile Path { get; set; }

        public string UserId { get; set; }
    }
}
