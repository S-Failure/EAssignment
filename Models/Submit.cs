using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Submit
    {
        public int SubmitId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int DivisionId { get; set; }

        public string SubmitPath { get; set; }

        public string SubmitName { get; set; }

        public string UserId { get; set; }
    }
}
