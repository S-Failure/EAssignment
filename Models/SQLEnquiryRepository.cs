using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class SQLEnquiryRepository : IEnquiryRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLEnquiryRepository> logger;

        public SQLEnquiryRepository(AppDbContext context,
                                ILogger<SQLEnquiryRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Enquiry Add(Enquiry enquiry)
        {
            context.Enquiries.Add(enquiry);
            context.SaveChanges();
            return enquiry;
        }

        public Enquiry Delete(int Id)
        {
            Enquiry enquiry = context.Enquiries.Find(Id);
            if (enquiry != null)
            {
                context.Enquiries.Remove(enquiry);
                context.SaveChanges();
            }
            return enquiry;
        }

        public IEnumerable<Enquiry> GetAllEnquiry()
        {
            return context.Enquiries;
        }

        public Enquiry GetEnquiry(int Id)
        {
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            return context.Enquiries.Find(Id);
        }

        public Enquiry Update(Enquiry enquiryChanges)
        {
            var enquiry = context.Enquiries.Attach(enquiryChanges);
            enquiry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return enquiryChanges;
        }
    }
}
