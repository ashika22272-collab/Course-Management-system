
using Microsoft.AspNetCore.Http;

namespace CourseManagementAPI.DTOs
{
	public class SubmissionUploadRequest
	{
		public int AssignmentId { get; set; }

		public int UserId { get; set; }

		public int CourseId { get; set; }

		public IFormFile? File { get; set; }
	}
}

