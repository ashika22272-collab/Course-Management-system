using System.ComponentModel.DataAnnotations;

namespace Course_Management.Models
{
	public class Assignment
	{
		[Key]
		public int assignmentid { get; set; }

		public int userid { get; set; }

		public int courseid { get; set; }

		// Course name returned from Courses table
		public string? coursename { get; set; }

		public string? assignmenttitle { get; set; }

		public string? modifiedby { get; set; }

		public DateTime? modifiedat { get; set; }

		public DateTime? createdat { get; set; }

		public string? createdby { get; set; }

		public bool? isactive { get; set; }
	}
}