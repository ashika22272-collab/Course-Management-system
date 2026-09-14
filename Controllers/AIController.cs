using Course_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_Management.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AIController : ControllerBase
	{
		private readonly AIService _aiService;

		public AIController(AIService aiService)
		{
			_aiService = aiService;
		}

		[HttpPost("ask")]
		public async Task<IActionResult> AskAI([FromBody] AIRequest request)
		{
			try
			{
				// Check whether question was provided
				if (string.IsNullOrWhiteSpace(request.Question))
				{
					return BadRequest(new
					{
						Message = "Question is required."
					});
				}

				// Send question to Gemini through AIService
				var answer = await _aiService.AskAI(request.Question);

				// Return Gemini response
				return Ok(new
				{
					Question = request.Question,
					Answer = answer
				});
			}
			catch (Exception ex)
			{
				// Print complete error in Visual Studio / backend terminal
				Console.WriteLine("====================================");
				Console.WriteLine("          AI ASSISTANT ERROR        ");
				Console.WriteLine("====================================");
				Console.WriteLine(ex.ToString());
				Console.WriteLine("====================================");

				// Return the actual error to browser for debugging
				return StatusCode(500, new
				{
					Message = "AI Error",
					Error = ex.ToString()
				});
			}
		}
	}

	public class AIRequest
	{
		public string Question { get; set; } = string.Empty;
	}
}