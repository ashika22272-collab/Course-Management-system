using Course_Management.DTOs;
using Course_Management.Interface;
using Course_Management.Models;
using Microsoft.AspNetCore.Authorization;
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

		/// <summary>
		/// Initializes a new instance of the UsersController class.
		/// </summary>
		/// <param name="userService">Service used to perform user operations.</param>
		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Retrieves all users.
		/// </summary>
		/// <returns>A list of all registered users.</returns>
		/// <response code="200">Returns the list of users.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="id">Unique identifier of the user.</param>
		/// <returns>User details.</returns>
		/// <response code="200">Returns the requested user.</response>
		/// <response code="404">User not found.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="request">Search parameters.</param>
		/// <returns>A list of matching users.</returns>
		/// <response code="200">Returns matching users.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="userDto">User information.</param>
		/// <returns>Success message.</returns>
		/// <response code="200">User created successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="id">Unique identifier of the user.</param>
		/// <param name="userDto">Updated user information.</param>
		/// <returns>No content.</returns>
		/// <response code="204">User updated successfully.</response>
		/// <response code="404">User not found.</response>
		/// <response code="500">Internal server error.</response>
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
		/// Deletes a user by ID.
		/// </summary>
		/// <param name="id">Unique identifier of the user.</param>
		/// <returns>No content.</returns>
		/// <response code="204">User deleted successfully.</response>
		/// <response code="404">User not found.</response>
		/// <response code="500">Internal server error.</response>
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