using Microsoft.AspNetCore.Mvc;
using Course_Management.Models;
using Course_Management.Services;

namespace Course_Management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly UserService _userService;

		public UsersController(UserService userService)
		{
			_userService = userService;
		}

		// GET: api/Users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			return Ok(await _userService.GetUsers());
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _userService.GetUser(id);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		// POST: api/Users
		[HttpPost]
		public async Task<ActionResult<User>> PostUser(User user)
		{
			var createdUser = await _userService.CreateUser(user);

			return CreatedAtAction(nameof(GetUser),
				new { id = createdUser.userid }, createdUser);
		}

		// PUT: api/Users/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(int id, User user)
		{
			if (id != user.userid)
			{
				return BadRequest();
			}

			var existingUser = await _userService.GetUser(id);

			if (existingUser == null)
			{
				return NotFound();
			}

			await _userService.UpdateUser(user);

			return NoContent();
		}

		// DELETE: api/Users/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _userService.GetUser(id);

			if (user == null)
			{
				return NotFound();
			}

			await _userService.DeleteUser(id);

			return NoContent();
		}
	}
}