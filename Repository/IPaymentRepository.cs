using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public interface IPaymentRepository
	{
		Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStudentId(
			int userId
		);
	}
}