using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Forum
    {
        public int ForumId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int DivisionId { get; set; }

        public string ForumData { get; set; }
        public string UserId { get; set; }
    }
}
