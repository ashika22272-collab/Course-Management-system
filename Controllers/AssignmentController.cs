using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Controllers
{
	/// <summary>
	/// Provides APIs for managing assignments.
	/// </summary>
	/// <remarks>
	/// All endpoints in this controller require JWT authentication.
	/// </remarks>
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AssignmentController : ControllerBase
	{
		private readonly IAssignmentRepository _assignmentRepository;

		/// <summary>
		/// Initializes a new instance of the AssignmentController class.
		/// </summary>
		/// <param name="assignmentRepository">
		/// Repository used to perform assignment operations.
		/// </param>
		public AssignmentController(IAssignmentRepository assignmentRepository)
		{
			_assignmentRepository = assignmentRepository;
		}

		/// <summary>
		/// Retrieves all assignments.
		/// </summary>
		/// <returns>A list of all assignments.</returns>
		/// <response code="200">Returns the list of assignments.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="id">Unique identifier of the assignment.</param>
		/// <returns>The requested assignment.</returns>
		/// <response code="200">Returns the requested assignment.</response>
		/// <response code="404">Assignment not found.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="assignment">Assignment information.</param>
		/// <returns>Success message.</returns>
		/// <response code="200">Assignment created successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="500">Internal server error.</response>
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
		/// <param name="assignment">Updated assignment information.</param>
		/// <returns>Success message.</returns>
		/// <response code="200">Assignment updated successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="500">Internal server error.</response>
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
					Message = "An error occurred while updating assignment.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Deletes an assignment by its ID.
		/// </summary>
		/// <param name="id">Unique identifier of the assignment.</param>
		/// <returns>Success message.</returns>
		/// <response code="200">Assignment deleted successfully.</response>
		/// <response code="404">Assignment not found.</response>
		/// <response code="500">Internal server error.</response>
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
					Message = "An error occurred while deleting assignment.",
					Error = ex.Message
				});
			}
		}
	}
}