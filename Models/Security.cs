using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class Security
    {
        public int SecurityId { get; set; }
        public int Length { get; set; }
        public int UniqueChars { get; set; }
        public bool ConfirmEmail { get; set; }
    }
}
