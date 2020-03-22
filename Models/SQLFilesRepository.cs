using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class SQLFilesRepository : IFilesRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLFilesRepository> logger;

        public SQLFilesRepository(AppDbContext context,
                                ILogger<SQLFilesRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Assignment AddAssignment(Assignment assignment)
        {
            context.Assignments.Add(assignment);
            context.SaveChanges();
            return assignment;
        }

        public Submit AddSubmit(Submit submit)
        {
            context.Submits.Add(submit);
            context.SaveChanges();
            return submit;
        }

        public Assignment DeleteAssignment(int Id)
        {
            Assignment assignment = context.Assignments.Find(Id);
            if (assignment != null)
            {
                context.Assignments.Remove(assignment);
                context.SaveChanges();
            }
            return assignment;
        }

        public Submit DeleteSubmit(int Id)
        {
            Submit submit = context.Submits.Find(Id);
            if (submit != null)
            {
                context.Submits.Remove(submit);
                context.SaveChanges();
            }
            return submit;
        }

        public IEnumerable<Assignment> GetAllAssignment()
        {
            return context.Assignments;
        }

        public IEnumerable<Submit> GetAllSubmit()
        {
            return context.Submits;
        }

        public Assignment GetAssignment(int Id)
        {
            return context.Assignments.Find(Id);
        }

        public Submit GetSubmit(int Id)
        {
            return context.Submits.Find(Id);
        }

        public Assignment UpdateAssignment(Assignment assignmentChanges)
        {
            var assignment = context.Assignments.Attach(assignmentChanges);
            assignment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return assignmentChanges;
        }

        public Submit UpdateSubmit(Submit submitChanges)
        {
            var submit = context.Submits.Attach(submitChanges);
            submit.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return submitChanges;
        }
    }
}
