using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int DivisionId { get; set; }

        public string AssignmentPath { get; set; }

        public string AssignmentName { get; set; }

        public string UserId { get; set; }
    }
}
