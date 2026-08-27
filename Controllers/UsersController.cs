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
	/// All endpoints in this controller require JWT authentication
	/// except user registration.
	/// </remarks>
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Gets all users.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetUsers()
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
		/// Gets a user by ID.
		/// </summary>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser(int id)
		{
			try
			{
				var user = await _userService.GetUserById(id);

				if (user == null)
				{
					return NotFound(new
					{
						Message = "User not found."
					});
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
		/// Creates a new user.
		/// </summary>
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> PostUser(UserDto userDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Check whether email already exists
				var existingUser = await _userService.GetUserByEmail(userDto.Email);

				if (existingUser != null)
				{
					return Conflict(new
					{
						Message = "Email already exists."
					});
				}

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
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var existingUser = await _userService.GetUserById(id);

				if (existingUser == null)
				{
					return NotFound(new
					{
						Message = "User not found."
					});
				}

				var user = new User
				{
					userid = id,
					firstname = userDto.FirstName,
					lastname = userDto.LastName,
					email = userDto.Email,
					phoneno = userDto.PhoneNo,
					age = userDto.Age,
					departmentid = userDto.DepartmentId,
					role = userDto.Role,
					password = userDto.Password,
					modified_by = "Admin",
					modified_at = DateTime.Now
				};

				await _userService.UpdateUser(user);

				return Ok(new
				{
					Message = "User updated successfully."
				});
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
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				var existingUser = await _userService.GetUserById(id);

				if (existingUser == null)
				{
					return NotFound(new
					{
						Message = "User not found."
					});
				}

				await _userService.DeleteUser(id);

				return Ok(new
				{
					Message = "User deleted successfully."
				});
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