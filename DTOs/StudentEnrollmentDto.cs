namespace CourseManagementAPI.DTOs
{
	public class StudentEnrollmentDto
	{
		public int EnrollmentId { get; set; }

		public int CourseId { get; set; }

		public string? CourseName { get; set; }

		public string? Description { get; set; }

		public string? Category { get; set; }

		public string? Instructor { get; set; }

		public int? Duration { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public decimal? Fees { get; set; }

		public DateTime? EnrollmentDate { get; set; }

		public string? Status { get; set; }
	}
}