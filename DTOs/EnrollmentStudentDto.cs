namespace CourseManagementAPI.DTOs
{
	public class EnrollmentStudentDto
	{
		public int UserID { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string? PhoneNo { get; set; }

		public DateTime? EnrollmentDate { get; set; }

		public string? Status { get; set; }
	}
}