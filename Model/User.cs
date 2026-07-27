using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Course_Management.Models
{
	public class User
	{
		[Key]
		public int userid { get; set; }

		public string firstname { get; set; }

		public string lastname { get; set; }

		public string email { get; set; }

		public string? phoneno { get; set; }

		public int? age { get; set; }

		public int? departmentid { get; set; }

		public string? role { get; set; }

		public string password { get; set; }

		public DateTime? registered_date { get; set; }

		public string? status { get; set; }

		public string? modified_by { get; set; }

		public DateTime? modified_at { get; set; }

		public string? created_by { get; set; }

		public DateTime? created_at { get; set; }

		public bool? is_active { get; set; }
	}
}