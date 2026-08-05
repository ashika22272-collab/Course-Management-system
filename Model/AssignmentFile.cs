namespace Course_Management.Models
{
	public class AssignmentFile
	{
		public int Id { get; set; }

		public string AssignmentName { get; set; } = string.Empty;

		public string FileName { get; set; } = string.Empty;

		public string FilePath { get; set; } = string.Empty;

		public DateTime UploadedDate { get; set; }
	}
}