using Microsoft.AspNetCore.Http;

namespace Course_Management.DTOs
{
    public class UploadAssignmentDto
    {
        public string AssignmentName { get; set; } = string.Empty;

        public IFormFile File { get; set; } = default!;
    }
}