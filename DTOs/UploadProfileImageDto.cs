using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Course_Management.DTOs
{
	public class UploadProfileImageDto
	{
		[Required]
		public IFormFile Image { get; set; } = null!;
	}
}