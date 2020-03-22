using EAssignment.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.ViewModels
{
    public class SendEnquiryViewModel
    {
        public int ID { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain: "gmail.com", ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Subject cannot exceed 50 characters")]
        public string Subject { get; set; }

        [Required]
        [MaxLength(252, ErrorMessage = "Message cannot exceed 252 characters")]
        public string Message { get; set; }

        public string  Reply { get; set; }
    }
}
