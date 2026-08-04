using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Course_Management.DTOs;
using Course_Management.Interface;
using Course_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Course_Management.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IConfiguration _configuration;

		public UsersController(IUserService userService, IConfiguration configuration)
		{
			_userService = userService;
			_configuration = configuration;
		}

		// ==========================
		// LOGIN
		// ==========================
		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestDto login)
		{
			try
			{
				var user = await _userService.Login(login.Email, login.Password);

				if (user == null)
				{
					return Unauthorized(new
					{
						Message = "Invalid Email or Password"
					});
				}

				var claims = new List<Claim>
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.email ?? ""),
					new Claim(JwtRegisteredClaimNames.Email, user.email ?? ""),
					new Claim(ClaimTypes.Name, user.firstname ?? ""),
					new Claim(ClaimTypes.Role, user.role ?? ""),
					new Claim("UserId", user.userid.ToString())
				};

				var key = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

				var credentials = new SigningCredentials(
					key,
					SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					claims: claims,
					expires: DateTime.Now.AddHours(2),
					signingCredentials: credentials);

				return Ok(new
				{
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					User = user
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "Error while logging in.",
					Error = ex.Message
				});
			}
		}

		// ==========================
		// GET ALL USERS
		// ==========================
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

		// ==========================
		// GET USER BY ID
		// ==========================
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

		// ==========================
		// SEARCH USERS
		// ==========================
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

		// ==========================
		// CREATE USER
		// ==========================
		[AllowAnonymous]
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

		// ==========================
		// UPDATE USER
		// ==========================
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

		// ==========================
		// DELETE USER
		// ==========================
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