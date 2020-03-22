using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public interface ICDSRepository
    {
        //Crud for Class
        Class GetClass(int Id);
        IEnumerable<Class> GetAllClass();
        Class AddClass(Class classes);
        Class UpdateClass(Class classChanges);
        Class DeleteClass(int Id);

        //Crud for Division
        Division GetDivision(int Id);
        IEnumerable<Division> GetAllDivision();
        Division AddDivision(Division division);
        Division UpdateDivision(Division divisionChanges);
        Division DeleteDivision(int Id);

        //Crud for Subject
        Subject GetSubject(int Id);
        IEnumerable<Subject> GetAllSubject();
        Subject AddSubject(Subject subject);
        Subject UpdateSubject(Subject subjectChanges);
        Subject DeleteSubject(int Id);

        //Crud for Security
        Security GetSecurity(int Id);
        IEnumerable<Security> GetAllSecurity();
        Security AddSecurity(Security security);
        Security UpdateSecurity(Security subjectChanges);
        Security DeleteSecurity(int Id);

        //Crud for Furum

        Forum GetForum(int Id);
        IEnumerable<Forum> GetAllForum();
        Forum AddForum(Forum forum);
        Forum UpdateForum(Forum forumChanges);
        Forum DeleteForum(int Id);

    }
}
