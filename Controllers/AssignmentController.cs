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
		[HttpGet]
		public async Task<IActionResult> GetAllAssignments()
		{
			try
			{
				var assignments = await _assignmentRepository.GetAllAssignments();
				return Ok(assignments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving assignments.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves an assignment by its ID.
		/// </summary>
		/// <param name="id">The ID of the assignment.</param>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAssignmentById(int id)
		{
			try
			{
				var assignment = await _assignmentRepository.GetAssignmentById(id);

				if (assignment == null)
				{
					return NotFound("Assignment not found.");
				}

				return Ok(assignment);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving the assignment.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Creates a new assignment.
		/// </summary>
		/// <param name="assignment">The assignment details.</param>
		[HttpPost]
		public async Task<IActionResult> AddAssignment(Assignment assignment)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				await _assignmentRepository.AddAssignment(assignment);

				return Ok("Assignment added successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while creating the assignment.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Updates an existing assignment.
		/// </summary>
		/// <param name="assignment">The updated assignment details.</param>
		[HttpPut]
		public async Task<IActionResult> UpdateAssignment(Assignment assignment)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				await _assignmentRepository.UpdateAssignment(assignment);

				return Ok("Assignment updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while updating the assignment.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Deletes an assignment by its ID.
		/// </summary>
		/// <param name="id">The ID of the assignment to delete.</param>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAssignment(int id)
		{
			try
			{
				var assignment = await _assignmentRepository.GetAssignmentById(id);

				if (assignment == null)
				{
					return NotFound("Assignment not found.");
				}

				await _assignmentRepository.DeleteAssignment(id);

				return Ok("Assignment deleted successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while deleting the assignment.",
					Error = ex.Message
				});
			}
		}
	}
}