using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ClassId { get; set; }
        public int DivisionId { get; set; }
        public int SubjectId { get; set; }
        public string DOB { get; set; }
    }
}
