
using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public interface ISubmissionRepository
	{
		Task<IEnumerable<SubmissionDto>> GetSubmissionsByStudentId(int userId);

		Task<SubmissionDto?> GetSubmissionByAssignmentAndStudent(
			int assignmentId,
			int userId
		);

		Task<int> CreateSubmission(SubmissionDto submission);

		Task<bool> UpdateSubmission(SubmissionDto submission);
	}
}

