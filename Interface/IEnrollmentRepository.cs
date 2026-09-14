using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public interface IEnrollmentRepository
	{
		// =====================================================
		// GET ENROLLED STUDENTS BY COURSE
		// =====================================================

		Task<IEnumerable<EnrollmentStudentDto>> GetStudentsByCourseId(
			int courseId
		);


		// =====================================================
		// GET ALL STUDENTS FOR INSTRUCTOR
		// =====================================================

		Task<IEnumerable<EnrollmentStudentDto>> GetStudentsByInstructorId(
			int instructorId
		);


		// =====================================================
		// GET COURSES ENROLLED BY STUDENT
		// =====================================================

		Task<IEnumerable<StudentEnrollmentDto>> GetEnrollmentsByStudentId(
			int userId
		);


		// =====================================================
		// ENROLL STUDENT
		// =====================================================

		Task<bool> EnrollStudent(
			int courseId,
			int userId
		);
	}
}