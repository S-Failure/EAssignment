using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Division
    {
        public int DivisionId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }
    }
}
