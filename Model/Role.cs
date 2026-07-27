using System.ComponentModel.DataAnnotations;

namespace Course_Management.Models
{
	public class Role
	{
		[Key]
		public int roleid { get; set; }

		public string rolename { get; set; } = string.Empty;

		public string? modified_by { get; set; }

		public DateTime? modified_at { get; set; }

		public string? created_by { get; set; }

		public DateTime? created_at { get; set; }

		public bool? is_active { get; set; }
	}
}