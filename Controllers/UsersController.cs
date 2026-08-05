using Course_Management.DTOs;
using Course_Management.Interface;
using Course_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Course_Management.Controllers
{
	/// <summary>
	/// Provides APIs for managing users.
	/// </summary>
	/// <remarks>
	/// All endpoints in this controller require JWT authentication.
	/// </remarks>
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IWebHostEnvironment _environment;

		/// <summary>
		/// Initializes a new instance of the UsersController class.
		/// </summary>
		/// <param name="userService">Service used to perform user operations.</param>
		/// <param name="environment">Web host environment.</param>
		public UsersController(
			IUserService userService,
			IWebHostEnvironment environment)
		{
			_userService = userService;
			_environment = environment;
		}

		/// <summary>
		/// Retrieves all users.
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			try
			{
				var users = await _userService.GetAllUsers();
				return Ok(users);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving users.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves a user by ID.
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			try
			{
				var user = await _userService.GetUserById(id);

				if (user == null)
				{
					return NotFound("User not found.");
				}

				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving the user.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Searches users based on the specified criteria.
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] UserSearchRequest request)
		{
			try
			{
				var users = await _userService.SearchUsers(request);
				return Ok(users);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while searching users.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Creates a new user.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> PostUser(UserDto userDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var user = new User
				{
					firstname = userDto.FirstName,
					lastname = userDto.LastName,
					email = userDto.Email,
					phoneno = userDto.PhoneNo,
					age = userDto.Age,
					departmentid = userDto.DepartmentId,
					role = userDto.Role,
					password = userDto.Password,
					registered_date = DateTime.Now,
					status = "Active",
					created_by = "Admin",
					created_at = DateTime.Now,
					modified_by = "Admin",
					modified_at = DateTime.Now,
					is_active = true
				};

				await _userService.AddUser(user);

				return Ok(new
				{
					Message = "User created successfully."
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while creating the user.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Updates an existing user.
		/// </summary>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(int id, UserDto userDto)
		{
			try
			{
				var existingUser = await _userService.GetUserById(id);

				if (existingUser == null)
					return NotFound();

				existingUser.firstname = userDto.FirstName;
				existingUser.lastname = userDto.LastName;
				existingUser.email = userDto.Email;
				existingUser.phoneno = userDto.PhoneNo;
				existingUser.age = userDto.Age;
				existingUser.departmentid = userDto.DepartmentId;
				existingUser.role = userDto.Role;
				existingUser.password = userDto.Password;
				existingUser.modified_at = DateTime.Now;

				await _userService.UpdateUser(existingUser);

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while updating the user.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Uploads a profile image.
		/// </summary>
		[HttpPost("upload-profile")]
		public async Task<IActionResult> UploadProfileImage([FromForm] UploadProfileImageDto request)
		{
			try
			{
				if (request.Image == null || request.Image.Length == 0)
				{
					return BadRequest("Please select an image.");
				}

				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

				var extension = Path.GetExtension(request.Image.FileName).ToLower();

				if (!allowedExtensions.Contains(extension))
				{
					return BadRequest("Only JPG, JPEG and PNG files are allowed.");
				}

				if (request.Image.Length > 2 * 1024 * 1024)
				{
					return BadRequest("Maximum file size is 2 MB.");
				}

				var fileName = $"{Guid.NewGuid()}{extension}";

				var uploadPath = Path.Combine(
					_environment.ContentRootPath,
					"Uploads",
					"ProfileImages");

				if (!Directory.Exists(uploadPath))
				{
					Directory.CreateDirectory(uploadPath);
				}

				var filePath = Path.Combine(uploadPath, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await request.Image.CopyToAsync(stream);
				}

				return Ok(new
				{
					Message = "Profile image uploaded successfully.",
					FileName = fileName,
					ImageUrl = $"/Uploads/ProfileImages/{fileName}"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while uploading the profile image.",
					Error = ex.Message
				});
			}
		}
		/// <summary>
		/// Retrieves an uploaded profile image.
		/// </summary>
		/// <param name="fileName">Image file name.</param>
		/// <returns>Image URL.</returns>
		[HttpGet("profile/{fileName}")]
		public IActionResult GetProfileImage(string fileName)
		{
			var filePath = Path.Combine(
				_environment.ContentRootPath,
				"Uploads",
				"ProfileImages",
				fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound(new
				{
					Message = "Image not found."
				});
			}

			return Ok(new
			{
				FileName = fileName,
				ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/ProfileImages/{fileName}"
			});
		}
		/// <summary>
		/// Deletes an uploaded profile image.
		/// </summary>
		/// <param name="fileName">Image file name.</param>
		/// <returns>Success message.</returns>
		[HttpDelete("profile/{fileName}")]
		public IActionResult DeleteProfileImage(string fileName)
		{
			try
			{
				var filePath = Path.Combine(
					_environment.ContentRootPath,
					"Uploads",
					"ProfileImages",
					fileName);

				if (!System.IO.File.Exists(filePath))
				{
					return NotFound(new
					{
						Message = "Image not found."
					});
				}

				System.IO.File.Delete(filePath);

				return Ok(new
				{
					Message = "Profile image deleted successfully."
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while deleting the profile image.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Deletes a user by ID.
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				var user = await _userService.GetUserById(id);

				if (user == null)
					return NotFound();

				await _userService.DeleteUser(id);

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while deleting the user.",
					Error = ex.Message
				});
			}
		}

	}
}