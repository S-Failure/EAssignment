using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Models
{
    public interface IFilesRepository
    {
        //Crud for Submit
        Submit GetSubmit(int Id);
        IEnumerable<Submit> GetAllSubmit();
        Submit AddSubmit(Submit submit);
        Submit UpdateSubmit(Submit submitChanges);
        Submit DeleteSubmit(int Id);

        //Crud for Assignment
        Assignment GetAssignment(int Id);
        IEnumerable<Assignment> GetAllAssignment();
        Assignment AddAssignment(Assignment assignment);
        Assignment UpdateAssignment(Assignment assignmentChanges);
        Assignment DeleteAssignment(int Id);
    }
}
