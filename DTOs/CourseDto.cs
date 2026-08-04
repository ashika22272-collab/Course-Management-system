using System.ComponentModel.DataAnnotations;

namespace Course_Management.DTOs
{
	public class CourseRequestDto
	{
		[Required(ErrorMessage = "Course Name is required.")]
		[MinLength(3, ErrorMessage = "Course Name must be at least 3 characters.")]
		[MaxLength(100, ErrorMessage = "Course Name cannot exceed 100 characters.")]
		public string CourseName { get; set; } = string.Empty;


		public string? Description { get; set; }


		[Required(ErrorMessage = "Department is required.")]
		public int? StdId { get; set; }


		public int? UserId { get; set; }


		[Required(ErrorMessage = "Duration is required.")]
		[Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
		public int? Duration { get; set; }


		[Required(ErrorMessage = "Fee is required.")]
		[Range(100, 100000, ErrorMessage = "Fee must be between 100 and 100000.")]
		public decimal? Fees { get; set; }


		public string? Status { get; set; }


		public DateTime StartDate { get; set; }


		public DateTime EndDate { get; set; }
	}
}