namespace YourProject.Models
{
	public class Payment
	{
		public int PayID { get; set; }

		public int? UserID { get; set; }

		public int? CourseID { get; set; }

		public string? ModifiedBy { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public DateTime? CreatedAt { get; set; }

		public string? CreatedBy { get; set; }

		public bool? IsActive { get; set; }

		public decimal? PaymentAmount { get; set; }

		public DateTime? PaymentDate { get; set; }

		public string? PaymentStatus { get; set; }
	}
}