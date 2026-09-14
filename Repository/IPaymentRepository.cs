
using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public interface IPaymentRepository
	{
		Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStudentId(
			int userId
		);

		Task<IEnumerable<InstructorPaymentDto>> GetPaymentsByInstructorId(
			int instructorId
		);
	}
}
