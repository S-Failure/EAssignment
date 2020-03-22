using EAssignment.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Enquiry
    {
        public int ID { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Reply { get; set; }

    }
}
