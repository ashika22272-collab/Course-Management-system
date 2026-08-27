using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public interface IEnrollmentRepository
	{
		// Get enrolled students by course
		Task<IEnumerable<EnrollmentStudentDto>> GetStudentsByCourseId(
			int courseId
		);

		// Get courses enrolled by student
		Task<IEnumerable<StudentEnrollmentDto>> GetEnrollmentsByStudentId(
			int userId
		);

		// Enroll student
		Task<bool> EnrollStudent(
			int courseId,
			int userId
		);
	}
}