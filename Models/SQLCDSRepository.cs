using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public class SQLCDSRepository : ICDSRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLCDSRepository> logger;

        public SQLCDSRepository(AppDbContext context,
                                ILogger<SQLCDSRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Class AddClass(Class classes)
        {
            context.Classes.Add(classes);
            context.SaveChanges();
            return classes;
        }

        public Division AddDivision(Division division)
        {
            context.Divisions.Add(division);
            context.SaveChanges();
            return division;
        }

        public Forum AddForum(Forum forum)
        {
            context.Forums.Add(forum);
            context.SaveChanges();
            return forum;
        }

        public Security AddSecurity(Security security)
        {
            context.Securities.Add(security);
            context.SaveChanges();
            return security;
        }

        public Subject AddSubject(Subject subject)
        {
            context.Subjects.Add(subject);
            context.SaveChanges();
            return subject;
        }

        public Class DeleteClass(int Id)
        {
            Class classes = context.Classes.Find(Id);
            if (classes != null)
            {
                context.Classes.Remove(classes);
                context.SaveChanges();
            }
            return classes;
        }

        public Division DeleteDivision(int Id)
        {
            Division division = context.Divisions.Find(Id);
            if (division != null)
            {
                context.Divisions.Remove(division);
                context.SaveChanges();
            }
            return division;
        }

        public Forum DeleteForum(int Id)
        {
            Forum forum = context.Forums.Find(Id);
            if (forum != null)
            {
                context.Forums.Remove(forum);
                context.SaveChanges();
            }
            return forum;
        }

        public Security DeleteSecurity(int Id)
        {
            Security security = context.Securities.Find(Id);
            if (security != null)
            {
                context.Securities.Remove(security);
                context.SaveChanges();
            }
            return security;
        }

        public Subject DeleteSubject(int Id)
        {
            Subject subject = context.Subjects.Find(Id);
            if (subject != null)
            {
                context.Subjects.Remove(subject);
                context.SaveChanges();
            }
            return subject;
        }

        public IEnumerable<Class> GetAllClass()
        {
            return context.Classes;
        }

        public IEnumerable<Division> GetAllDivision()
        {
            return context.Divisions;
        }

        public IEnumerable<Forum> GetAllForum()
        {
            return context.Forums;
        }

        public IEnumerable<Security> GetAllSecurity()
        {
            return context.Securities;
        }

        public IEnumerable<Subject> GetAllSubject()
        {
            return context.Subjects;
        }

        public Class GetClass(int Id)
        {
            return context.Classes.Find(Id);
        }

        public Division GetDivision(int Id)
        {
            return context.Divisions.Find(Id);
        }

        public Forum GetForum(int Id)
        {
            return context.Forums.Find(Id);
        }

        public Security GetSecurity(int Id)
        {
            return context.Securities.Find(Id);
        }

        public Subject GetSubject(int Id)
        {
            return context.Subjects.Find(Id);
        }

        public Class UpdateClass(Class classChanges)
        {
            var classes = context.Classes.Attach(classChanges);
            classes.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return classChanges;
        }

        public Division UpdateDivision(Division divisionChanges)
        {
            var division = context.Divisions.Attach(divisionChanges);
            division.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return divisionChanges;
        }

        public Forum UpdateForum(Forum forumChanges)
        {
            var forum = context.Forums.Attach(forumChanges);
            forum.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return forumChanges;
        }

        public Security UpdateSecurity(Security subjectChanges)
        {
            var security = context.Securities.Attach(subjectChanges);
            security.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return subjectChanges;
        }

        public Forum UpdateSecurity(Forum forumChanges)
        {
            var forum = context.Forums.Attach(forumChanges);
            forum.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return forumChanges;
        }

        public Subject UpdateSubject(Subject subjectChanges)
        {
            var subject = context.Subjects.Attach(subjectChanges);
            subject.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return subjectChanges;
        }
    }
}
