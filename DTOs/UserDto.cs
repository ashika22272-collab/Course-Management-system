using System.ComponentModel.DataAnnotations;

namespace Course_Management.DTOs
{
	public class UserDto
	{
		[Required(ErrorMessage = "First Name is required.")]
		[MinLength(3, ErrorMessage = "First Name must be at least 3 characters.")]
		[MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
		public string FirstName { get; set; } = string.Empty;


		[Required(ErrorMessage = "Last Name is required.")]
		[MinLength(3, ErrorMessage = "Last Name must be at least 3 characters.")]
		[MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
		public string LastName { get; set; } = string.Empty;


		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; } = string.Empty;


		[Required(ErrorMessage = "Phone Number is required.")]
		[Phone(ErrorMessage = "Invalid phone number format.")]
		public string PhoneNo { get; set; } = string.Empty;


		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8, ErrorMessage = "Password must be minimum 8 characters.")]
		public string Password { get; set; } = string.Empty;


		[Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
		public int Age { get; set; }


		[Required(ErrorMessage = "Department is required.")]
		public int DepartmentId { get; set; }


		[Required(ErrorMessage = "Role is required.")]
		public string Role { get; set; } = string.Empty;
	}
}