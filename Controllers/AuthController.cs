using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Course_Management.Interface;
using Course_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Course_Management.Controllers
{
	/// <summary>
	/// Provides authentication APIs for user login.
	/// </summary>
	/// <remarks>
	/// This controller is responsible for validating user credentials
	/// and generating JWT access tokens for authenticated users.
	/// </remarks>
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the AuthController class.
		/// </summary>
		/// <param name="userService">Service used to validate user credentials.</param>
		/// <param name="configuration">Application configuration containing JWT settings.</param>
		public AuthController(
			IUserService userService,
			IConfiguration configuration)
		{
			_userService = userService;
			_configuration = configuration;
		}

		/// <summary>
		/// Authenticates a user and generates a JWT token.
		/// </summary>
		/// <param name="request">User login credentials.</param>
		/// <returns>JWT token and authenticated user information.</returns>
		/// <response code="200">Login successful.</response>
		/// <response code="400">Email or password was not provided.</response>
		/// <response code="401">Invalid email or password.</response>
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Email) ||
				string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest(new
				{
					Message = "Email and password are required."
				});
			}

			var user = await _userService.Login(
				request.Email,
				request.Password);

			if (user == null)
			{
				return Unauthorized(new
				{
					Message = "Invalid email or password."
				});
			}

			var token = GenerateJwtToken(user);

			return Ok(new
			{
				Message = "Login successful.",
				Token = token,
				User = new
				{
					UserId = user.userid,
					FirstName = user.firstname,
					LastName = user.lastname,
					Email = user.email,
					PhoneNo = user.phoneno,
					Age = user.age,
					DepartmentId = user.departmentid,
					Role = user.role,
					Status = user.status
				}
			});
		}

		/// <summary>
		/// Generates a JWT token for an authenticated user.
		/// </summary>
		/// <param name="user">Authenticated user.</param>
		/// <returns>A signed JWT token.</returns>
		private string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.email ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.Email, user.email ?? string.Empty),
				new Claim(ClaimTypes.Name, user.firstname ?? string.Empty),
				new Claim(ClaimTypes.Role, user.role ?? string.Empty),
				new Claim("UserId", user.userid.ToString())
			};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
			);

			var credentials = new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256
			);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}