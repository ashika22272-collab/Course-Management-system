using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Course_Management.Models;
using Course_Management.Interface;
using Course_Management.DTOs;

namespace Course_Management.Controllers
{
	/// <summary>
	/// Provides APIs for managing courses.
	/// </summary>
	/// <remarks>
	/// All endpoints in this controller require JWT authentication.
	/// </remarks>
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CourseController : ControllerBase
	{
		private readonly ICourseRepository _courseRepository;

		/// <summary>
		/// Initializes a new instance of the CourseController class.
		/// </summary>
		/// <param name="courseRepository">
		/// Repository used to perform course operations.
		/// </param>
		public CourseController(ICourseRepository courseRepository)
		{
			_courseRepository = courseRepository;
		}

		/// <summary>
		/// Retrieves all courses.
		/// </summary>
		/// <returns>A list of all available courses.</returns>
		/// <response code="200">Returns the list of courses.</response>
		/// <response code="500">Internal server error.</response>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
		{
			try
			{
				var courses = await _courseRepository.GetAllCourses();
				return Ok(courses);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving courses.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Retrieves a course by its ID.
		/// </summary>
		/// <param name="id">Unique identifier of the course.</param>
		/// <returns>The requested course.</returns>
		/// <response code="200">Returns the requested course.</response>
		/// <response code="404">Course not found.</response>
		/// <response code="500">Internal server error.</response>
		[HttpGet("{id}")]
		public async Task<ActionResult<Course>> GetCourse(int id)
		{
			try
			{
				var course = await _courseRepository.GetCourseById(id);

				if (course == null)
				{
					return NotFound("Course not found.");
				}

				return Ok(course);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while retrieving the course.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Creates a new course.
		/// </summary>
		/// <param name="courseDto">Course information.</param>
		/// <returns>The newly created course.</returns>
		/// <response code="201">Course created successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="500">Internal server error.</response>
		[HttpPost]
		public async Task<ActionResult<Course>> PostCourse(CourseRequestDto courseDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var course = new Course
				{
					coursename = courseDto.CourseName,
					description = courseDto.Description,
					stdid = courseDto.StdId,
					userid = courseDto.UserId,
					start_date = courseDto.StartDate,
					end_date = courseDto.EndDate,
					fees = courseDto.Fees,
					status = courseDto.Status,
					created_by = "Admin",
					created_at = DateTime.Now,
					modified_by = "Admin",
					modified_at = DateTime.Now,
					is_active = true
				};

				var createdCourse = await _courseRepository.AddCourse(course);

				return CreatedAtAction(
					nameof(GetCourse),
					new { id = createdCourse.courseid },
					createdCourse
				);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while creating the course.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Updates an existing course.
		/// </summary>
		/// <param name="id">Unique identifier of the course.</param>
		/// <param name="courseDto">Updated course information.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Course updated successfully.</response>
		/// <response code="400">Invalid request data.</response>
		/// <response code="404">Course not found.</response>
		/// <response code="500">Internal server error.</response>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCourse(int id, CourseRequestDto courseDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var existingCourse = await _courseRepository.GetCourseById(id);

				if (existingCourse == null)
				{
					return NotFound("Course not found.");
				}

				existingCourse.coursename = courseDto.CourseName;
				existingCourse.description = courseDto.Description;
				existingCourse.stdid = courseDto.StdId;
				existingCourse.userid = courseDto.UserId;
				existingCourse.start_date = courseDto.StartDate;
				existingCourse.end_date = courseDto.EndDate;
				existingCourse.fees = courseDto.Fees;
				existingCourse.status = courseDto.Status;
				existingCourse.modified_by = "Admin";
				existingCourse.modified_at = DateTime.Now;
				existingCourse.is_active = true;

				await _courseRepository.UpdateCourse(existingCourse);

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while updating the course.",
					Error = ex.Message
				});
			}
		}

		/// <summary>
		/// Deletes a course by its ID.
		/// </summary>
		/// <param name="id">Unique identifier of the course.</param>
		/// <returns>No content.</returns>
		/// <response code="204">Course deleted successfully.</response>
		/// <response code="404">Course not found.</response>
		/// <response code="500">Internal server error.</response>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			try
			{
				var course = await _courseRepository.GetCourseById(id);

				if (course == null)
				{
					return NotFound("Course not found.");
				}

				await _courseRepository.DeleteCourse(id);

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Message = "An error occurred while deleting the course.",
					Error = ex.Message
				});
			}
		}
	}
}