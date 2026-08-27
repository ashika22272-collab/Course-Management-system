namespace CourseManagementAPI.DTOs
{
	public class PaymentResponseDto
	{
		public int PaymentId { get; set; }

		public string CourseName { get; set; } = string.Empty;

		public DateTime? PaymentDate { get; set; }

		public decimal Amount { get; set; }

		public string PaymentStatus { get; set; } = string.Empty;
	}
}