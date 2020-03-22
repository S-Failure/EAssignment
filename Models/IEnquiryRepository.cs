using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public interface IEnquiryRepository
    {
        Enquiry GetEnquiry(int Id);
        IEnumerable<Enquiry> GetAllEnquiry();
        Enquiry Add(Enquiry enquiry);
        Enquiry Update(Enquiry enquiryChanges);
        Enquiry Delete(int Id);
    }
}
