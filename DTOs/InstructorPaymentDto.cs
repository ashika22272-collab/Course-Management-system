
namespace CourseManagementAPI.DTOs
{
	public class InstructorPaymentDto
	{
		public int PaymentId { get; set; }

		public int UserID { get; set; }

		public string StudentName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string CourseName { get; set; } = string.Empty;

		public DateTime? PaymentDate { get; set; }

		public decimal Amount { get; set; }

		public string PaymentStatus { get; set; } = string.Empty;
	}
}
