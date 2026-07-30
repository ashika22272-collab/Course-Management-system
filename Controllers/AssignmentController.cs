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

		/// <summary>
		/// Retrieves all assignments.
		/// </summary>
		/// <returns>Returns a list of all assignments.</returns>
		// GET: api/Assignment
		[HttpGet]
		public async Task<IActionResult> GetAllAssignments()
		{
			var assignments = await _assignmentRepository.GetAllAssignments();
			return Ok(assignments);
		}

		/// <summary>
		/// Retrieves an assignment by its ID.
		/// </summary>
		/// <param name="id">The ID of the assignment.</param>
		/// <returns>Returns the assignment if found; otherwise, returns NotFound.</returns>
		// GET: api/Assignment/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAssignmentById(int id)
		{
			var assignment = await _assignmentRepository.GetAssignmentById(id);

			if (assignment == null)
				return NotFound();

			return Ok(assignment);
		}

		/// <summary>
		/// Creates a new assignment.
		/// </summary>
		/// <param name="assignment">The assignment details.</param>
		/// <returns>Returns a success message after creating the assignment.</returns>
		// POST: api/Assignment
		[HttpPost]
		public async Task<IActionResult> AddAssignment(Assignment assignment)
		{
			await _assignmentRepository.AddAssignment(assignment);

			return Ok("Assignment added successfully");
		}

		/// <summary>
		/// Updates an existing assignment.
		/// </summary>
		/// <param name="assignment">The updated assignment details.</param>
		/// <returns>Returns a success message after updating the assignment.</returns>
		// PUT: api/Assignment
		[HttpPut]
		public async Task<IActionResult> UpdateAssignment(Assignment assignment)
		{
			await _assignmentRepository.UpdateAssignment(assignment);

			return Ok("Assignment updated successfully");
		}

		/// <summary>
		/// Deletes an assignment by its ID.
		/// </summary>
		/// <param name="id">The ID of the assignment to delete.</param>
		/// <returns>Returns a success message after deleting the assignment.</returns>
		// DELETE: api/Assignment/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAssignment(int id)
		{
			await _assignmentRepository.DeleteAssignment(id);

			return Ok("Assignment deleted successfully");
		}
	}
}