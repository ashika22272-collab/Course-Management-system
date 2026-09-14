
namespace CourseManagementAPI.DTOs
{
	public class SubmissionDto
	{
		public int SubID { get; set; }

		public int? SubNo { get; set; }

		public string? SubGrade { get; set; }

		public string? Feedback { get; set; }

		public int? UserID { get; set; }

		public int? CourseID { get; set; }

		public int? AssignmentID { get; set; }

		public string? SubmissionStatus { get; set; }

		public string? FileName { get; set; }

		public string? FilePath { get; set; }

		public DateTime? CreatedAt { get; set; }

		public string? CreatedBy { get; set; }

		public bool? IsActive { get; set; }
	}
}
