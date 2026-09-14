
using System.Runtime.Intrinsics.Arm;
using Course_Management.Models;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CourseManagementAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class SubmissionController : ControllerBase
	{
		private readonly ISubmissionRepository _submissionRepository;
		private readonly IWebHostEnvironment _environment;

		public SubmissionController(
			ISubmissionRepository submissionRepository,
			IWebHostEnvironment environment)
		{
			_submissionRepository = submissionRepository;
			_environment = environment;
		}

		// GET: api/Submission/student/6
		[HttpGet("student/{userId}")]
		public async Task<IActionResult> GetSubmissionsByStudent(int userId)
		{
			try
			{
				if (userId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Student ID."
					});
				}

				var submissions =
					await _submissionRepository.GetSubmissionsByStudentId(userId);

				return Ok(submissions);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving submissions.",
					error = ex.Message
				});
			}
		}

		// GET: api/Submission/assignment/13/student/6
		[HttpGet("assignment/{assignmentId}/student/{userId}")]
		public async Task<IActionResult> GetSubmission(
			int assignmentId,
			int userId)
		{
			try
			{
				if (assignmentId <= 0 || userId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Assignment ID or Student ID."
					});
				}

				var submission =
					await _submissionRepository
						.GetSubmissionByAssignmentAndStudent(
							assignmentId,
							userId
						);

				if (submission == null)
				{
					return NotFound(new
					{
						message = "Assignment has not been submitted yet."
					});
				}

				return Ok(submission);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving the submission.",
					error = ex.Message
				});
			}
		}

		// POST: api/Submission/upload
		[HttpPost("upload")]
		[RequestSizeLimit(10 * 1024 * 1024)]
		public async Task<IActionResult> UploadSubmission(
			[FromForm] SubmissionUploadRequest request)
		{
			try
			{
				if (request.AssignmentId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Assignment ID."
					});
				}

				if (request.UserId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Student ID."
					});
				}

				if (request.CourseId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Course ID."
					});
				}

				if (request.File == null || request.File.Length == 0)
				{
					return BadRequest(new
					{
						message = "Please select a file to submit."
					});
				}

				// Maximum file size = 10 MB
				if (request.File.Length > 10 * 1024 * 1024)
				{
					return BadRequest(new
					{
						message = "File size must not exceed 10 MB."
					});
				}

				// Allowed file types
				var allowedExtensions = new[]
				{
					".pdf",
					".doc",
					".docx",
					".txt",
					".zip",
					".jpg",
					".jpeg",
					".png"
				};

				var extension =
					Path.GetExtension(request.File.FileName)
						.ToLowerInvariant();

				if (!allowedExtensions.Contains(extension))
				{
					return BadRequest(new
					{
						message =
							"Invalid file type. Allowed: PDF, DOC, DOCX, TXT, ZIP, JPG, JPEG, PNG."
					});
				}

				// Check whether the student already submitted this assignment
				var existingSubmission =
					await _submissionRepository
						.GetSubmissionByAssignmentAndStudent(
							request.AssignmentId,
							request.UserId
						);

				// Get wwwroot path
				var webRootPath =
					_environment.WebRootPath;

				if (string.IsNullOrWhiteSpace(webRootPath))
				{
					webRootPath = Path.Combine(
						Directory.GetCurrentDirectory(),
						"wwwroot"
					);
				}

				// Create upload folder
				var uploadFolder = Path.Combine(
					webRootPath,
					"uploads",
					"submissions"
				);

				if (!Directory.Exists(uploadFolder))
				{
					Directory.CreateDirectory(uploadFolder);
				}

				// Generate unique file name
				var uniqueFileName =
					$"{Guid.NewGuid()}{extension}";

				var physicalFilePath =
					Path.Combine(
						uploadFolder,
						uniqueFileName
					);

				// Save uploaded file
				using (var stream = new FileStream(
					physicalFilePath,
					FileMode.Create))
				{
					await request.File.CopyToAsync(stream);
				}

				// Path stored in database
				var relativeFilePath =
					$"/uploads/submissions/{uniqueFileName}";

				// =====================================================
				// RESUBMISSION
				// =====================================================
				if (existingSubmission != null)
				{
					// Delete old physical file
					if (!string.IsNullOrWhiteSpace(
						existingSubmission.FilePath))
					{
						var oldFilePath = Path.Combine(
							webRootPath,
							existingSubmission.FilePath
								.TrimStart('/')
								.Replace(
									"/",
									Path.DirectorySeparatorChar.ToString()
								)
						);

						if (System.IO.File.Exists(oldFilePath))
						{
							System.IO.File.Delete(oldFilePath);
						}
					}

					// Update existing submission
					existingSubmission.FileName =
						request.File.FileName;

					existingSubmission.FilePath =
						relativeFilePath;

					existingSubmission.CreatedBy =
						request.UserId.ToString();

					var updated =
						await _submissionRepository
							.UpdateSubmission(existingSubmission);

					if (!updated)
					{
						return StatusCode(500, new
						{
							message =
								"Unable to update the submission."
						});
					}

					return Ok(new
					{
						message =
							"Assignment resubmitted successfully.",

						submissionId =
							existingSubmission.SubID,

						assignmentId =
							request.AssignmentId,

						userId =
							request.UserId,

						fileName =
							request.File.FileName,

						status = "Submitted"
					});
				}

				// =====================================================
				// NEW SUBMISSION
				// =====================================================

				var submission = new SubmissionDto
				{
					UserID = request.UserId,
					CourseID = request.CourseId,
					AssignmentID = request.AssignmentId,
					SubmissionStatus = "Submitted",
					FileName = request.File.FileName,
					FilePath = relativeFilePath,
					CreatedBy = request.UserId.ToString(),
					IsActive = true
				};

				var submissionId =
					await _submissionRepository
						.CreateSubmission(submission);

				return Ok(new
				{
					message =
						"Assignment submitted successfully.",

					submissionId,

					assignmentId =
						request.AssignmentId,

					userId =
						request.UserId,

					fileName =
						request.File.FileName,

					status = "Submitted"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message =
						"An error occurred while submitting the assignment.",

					error = ex.Message
				});
			}
		}
	}
}
