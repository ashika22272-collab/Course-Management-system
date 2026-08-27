using System.ComponentModel.DataAnnotations;

namespace Course_Management.Models
{
	public class Course
	{
		[Key]
		public int courseid { get; set; }

		public string coursename { get; set; } = string.Empty;

		public string? description { get; set; }

		public int? stdid { get; set; }

		public int? userid { get; set; }

		public string? instructor { get; set; }

		public DateTime? start_date { get; set; }

		public DateTime? end_date { get; set; }

		public decimal? fees { get; set; }

		public string? status { get; set; }

		public string? modified_by { get; set; }

		public DateTime? modified_at { get; set; }

		public string? created_by { get; set; }

		public DateTime? created_at { get; set; }

		public bool? is_active { get; set; }

		public string? Category { get; set; }

		public int? Duration { get; set; }

		public int Enrolled { get; set; }
	}
}