using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface IAssignmentRepository
	{
		Task<IEnumerable<Assignment>> GetAllAssignments();

		Task<Assignment?> GetAssignmentById(int id);

		Task<int> AddAssignment(Assignment assignment);

		Task<int> UpdateAssignment(Assignment assignment);

		Task<int> DeleteAssignment(int id);
	}
}