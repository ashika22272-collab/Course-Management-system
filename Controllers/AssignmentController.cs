using Microsoft.AspNetCore.Mvc;
using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AssignmentController : ControllerBase
	{
		private readonly IAssignmentRepository _assignmentRepository;

		public AssignmentController(IAssignmentRepository assignmentRepository)
		{
			_assignmentRepository = assignmentRepository;
		}


		// GET: api/Assignment
		[HttpGet]
		public async Task<IActionResult> GetAllAssignments()
		{
			var assignments = await _assignmentRepository.GetAllAssignments();
			return Ok(assignments);
		}


		// GET: api/Assignment/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAssignmentById(int id)
		{
			var assignment = await _assignmentRepository.GetAssignmentById(id);

			if (assignment == null)
				return NotFound();

			return Ok(assignment);
		}


		// POST: api/Assignment
		[HttpPost]
		public async Task<IActionResult> AddAssignment(Assignment assignment)
		{
			await _assignmentRepository.AddAssignment(assignment);

			return Ok("Assignment added successfully");
		}


		// PUT: api/Assignment
		[HttpPut]
		public async Task<IActionResult> UpdateAssignment(Assignment assignment)
		{
			await _assignmentRepository.UpdateAssignment(assignment);

			return Ok("Assignment updated successfully");
		}


		// DELETE: api/Assignment/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAssignment(int id)
		{
			await _assignmentRepository.DeleteAssignment(id);

			return Ok("Assignment deleted successfully");
		}
	}
}