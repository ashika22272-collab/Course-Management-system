using Course_Management.DTOs;
using Course_Management.Interface;
using Course_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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
		private readonly IAssignmentFileRepository _assignmentFileRepository;
		private readonly IWebHostEnvironment _environment;

		/// <summary>
		/// Initializes a new instance of the AssignmentController class.
		/// </summary>
		public AssignmentController(
			IAssignmentRepository assignmentRepository,
			IAssignmentFileRepository assignmentFileRepository,
			IWebHostEnvironment environment)
		{
			_assignmentRepository = assignmentRepository;
			_assignmentFileRepository = assignmentFileRepository;
			_environment = environment;
		}

		/// <summary>
		/// Retrieves all assignments.
		/// </summary>
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
		[HttpPost]
		public async Task<IActionResult> AddAssignment(Assignment assignment)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

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
		/// Updates an assignment.
		/// </summary>
		[HttpPut]
		public async Task<IActionResult> UpdateAssignment(Assignment assignment)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

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
		/// Uploads an assignment PDF.
		/// </summary>
		[HttpPost("upload")]
		public async Task<IActionResult> UploadAssignment([FromForm] UploadAssignmentDto request)
		{
			try
			{
				if (request.File == null || request.File.Length == 0)
				{
					return BadRequest("Please select a PDF file.");
				}

				var extension = Path.GetExtension(request.File.FileName).ToLower();

				if (extension != ".pdf")
				{
					return BadRequest("Only PDF files are allowed.");
				}

				if (request.File.Length > 10 * 1024 * 1024)
				{
					return BadRequest("Maximum file size is 10 MB.");
				}

				var fileName = $"{Guid.NewGuid()}{extension}";

				var uploadFolder = Path.Combine(
					_environment.ContentRootPath,
					"Uploads",
					"Assignments");

				if (!Directory.Exists(uploadFolder))
				{
					Directory.CreateDirectory(uploadFolder);
				}

				var filePath = Path.Combine(uploadFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await request.File.CopyToAsync(stream);
				}

				var assignmentFile = new AssignmentFile
				{
					AssignmentName = request.AssignmentName,
					FileName = fileName,
					FilePath = filePath,
					UploadedDate = DateTime.Now
				};

				await _assignmentFileRepository.UploadAssignment(assignmentFile);

				return Ok(new
				{
					Message = "Assignment uploaded successfully.",
					FileName = fileName
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while uploading the assignment.",
					Error = ex.Message
				});
			}
		}
		/// <summary>
		/// Retrieves all uploaded assignment files.
		/// </summary>
		/// <returns>List of uploaded assignment files.</returns>
		[HttpGet("files")]
		public async Task<IActionResult> GetUploadedAssignments()
		{
			try
			{
				var files = await _assignmentFileRepository.GetAssignments();

				return Ok(files);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving uploaded assignment files.",
					Error = ex.Message
				});
			}
		}
		/// <summary>
		/// Downloads an uploaded assignment PDF.
		/// </summary>
		/// <param name="id">Assignment file ID.</param>
		/// <returns>PDF file.</returns>
		[HttpGet("download/{id}")]
		public async Task<IActionResult> DownloadAssignment(int id)
		{
			try
			{
				var assignment = await _assignmentFileRepository.GetAssignmentById(id);

				if (assignment == null)
				{
					return NotFound(new
					{
						Message = "Assignment file not found."
					});
				}

				if (!System.IO.File.Exists(assignment.FilePath))
				{
					return NotFound(new
					{
						Message = "File does not exist on the server."
					});
				}

				var fileBytes = await System.IO.File.ReadAllBytesAsync(assignment.FilePath);

				return File(
					fileBytes,
					"application/pdf",
					assignment.FileName);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while downloading the assignment.",
					Error = ex.Message
				});
			}
		}
		/// <summary>
		/// Deletes an uploaded assignment PDF.
		/// </summary>
		/// <param name="id">Assignment file ID.</param>
		/// <returns>Success message.</returns>
		[HttpDelete("file/{id}")]
		public async Task<IActionResult> DeleteUploadedAssignment(int id)
		{
			try
			{
				var assignment = await _assignmentFileRepository.GetAssignmentById(id);

				if (assignment == null)
				{
					return NotFound(new
					{
						Message = "Assignment file not found."
					});
				}

				if (System.IO.File.Exists(assignment.FilePath))
				{
					System.IO.File.Delete(assignment.FilePath);
				}

				await _assignmentFileRepository.DeleteAssignment(id);

				return Ok(new
				{
					Message = "Assignment deleted successfully."
				});
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

		/// <summary>
		/// Deletes an assignment.
		/// </summary>
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