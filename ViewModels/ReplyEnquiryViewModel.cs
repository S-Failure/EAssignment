using EAssignment.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class ReplyEnquiryViewModel
    {
        public int ID { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        [Required]
        [MaxLength(252, ErrorMessage = "Message cannot exceed 252 characters")]
        public string Reply { get; set; }
    }
}
