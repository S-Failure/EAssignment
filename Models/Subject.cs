using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        public string SubjectName { get; set; }
        public int ClassId { get; set; }
    }
}
