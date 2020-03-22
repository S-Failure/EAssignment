using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Class
    {
        [Required]
        public int ClassId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }
    }
}
