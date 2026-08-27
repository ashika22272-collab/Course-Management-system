using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class EnrollmentController : ControllerBase
	{
		private readonly IEnrollmentRepository _enrollmentRepository;

		public EnrollmentController(
			IEnrollmentRepository enrollmentRepository)
		{
			_enrollmentRepository = enrollmentRepository;
		}


		// =====================================================
		// GET ENROLLED STUDENTS BY COURSE
		// =====================================================

		[HttpGet("course/{courseId}")]
		public async Task<IActionResult> GetStudentsByCourse(
			int courseId)
		{
			try
			{
				if (courseId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Course ID."
					});
				}

				var students =
					await _enrollmentRepository
						.GetStudentsByCourseId(courseId);

				return Ok(students);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving enrolled students.",
					error = ex.Message
				});
			}
		}


		// =====================================================
		// GET MY ENROLLMENTS BY STUDENT ID
		// =====================================================

		[HttpGet("student/{userId}")]
		public async Task<IActionResult> GetEnrollmentsByStudent(
			int userId)
		{
			try
			{
				if (userId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid User ID."
					});
				}

				var enrollments =
					await _enrollmentRepository
						.GetEnrollmentsByStudentId(userId);

				return Ok(enrollments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while retrieving student enrollments.",
					error = ex.Message
				});
			}
		}


		// =====================================================
		// ENROLL STUDENT
		// =====================================================

		[HttpPost]
		public async Task<IActionResult> EnrollStudent(
			[FromBody] EnrollmentRequest request)
		{
			try
			{
				if (request == null)
				{
					return BadRequest(new
					{
						message = "Enrollment request is required."
					});
				}


				if (request.CourseId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid Course ID."
					});
				}


				if (request.UserId <= 0)
				{
					return BadRequest(new
					{
						message = "Invalid User ID."
					});
				}


				var enrolled =
					await _enrollmentRepository
						.EnrollStudent(
							request.CourseId,
							request.UserId
						);


				if (!enrolled)
				{
					return BadRequest(new
					{
						message =
							"Student is already enrolled in this course or the course does not exist."
					});
				}


				return Ok(new
				{
					message = "Student enrolled successfully.",
					courseId = request.CourseId,
					userId = request.UserId
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "An error occurred while enrolling the student.",
					error = ex.Message
				});
			}
		}
	}


	// =========================================================
	// REQUEST MODEL
	// =========================================================

	public class EnrollmentRequest
	{
		public int CourseId { get; set; }

		public int UserId { get; set; }
	}
}