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
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CourseController : ControllerBase
	{
		private readonly ICourseRepository _courseRepository;

		public CourseController(ICourseRepository courseRepository)
		{
			_courseRepository = courseRepository;
		}

		// =====================================================
		// GET ALL COURSES
		// =====================================================

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


		// =====================================================
		// GET COURSE BY ID
		// =====================================================

		[HttpGet("{id}")]
		public async Task<ActionResult<Course>> GetCourse(int id)
		{
			try
			{
				var course = await _courseRepository.GetCourseById(id);

				if (course == null)
				{
					return NotFound(new
					{
						Message = "Course not found."
					});
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


		// =====================================================
		// CREATE COURSE
		// =====================================================

		[HttpPost]
		public async Task<ActionResult<Course>> PostCourse(
			[FromBody] CourseRequestDto courseDto)
		{
			try
			{
				// ---------------------------------------------
				// VALIDATION
				// ---------------------------------------------

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}


				// ---------------------------------------------
				// DATE VALIDATION
				// ---------------------------------------------

				if (!courseDto.StartDate.HasValue)
				{
					return BadRequest(new
					{
						Message = "Start Date is required."
					});
				}

				if (!courseDto.EndDate.HasValue)
				{
					return BadRequest(new
					{
						Message = "End Date is required."
					});
				}


				if (courseDto.EndDate.Value < courseDto.StartDate.Value)
				{
					return BadRequest(new
					{
						Message = "End Date cannot be before Start Date."
					});
				}


				// ---------------------------------------------
				// CREATE COURSE OBJECT
				// ---------------------------------------------

				var course = new Course
				{
					coursename = courseDto.CourseName,
					description = courseDto.Description,

					stdid = courseDto.StdId,
					userid = courseDto.UserId,

					start_date = courseDto.StartDate.Value,
					end_date = courseDto.EndDate.Value,

					fees = courseDto.Fees,

					status = string.IsNullOrWhiteSpace(courseDto.Status)
						? "Active"
						: courseDto.Status,

					created_by = "Admin",
					created_at = DateTime.Now,

					modified_by = "Admin",
					modified_at = DateTime.Now,

					is_active = true
				};


				// ---------------------------------------------
				// SAVE TO DATABASE
				// ---------------------------------------------

				var createdCourse =
					await _courseRepository.AddCourse(course);


				// ---------------------------------------------
				// RETURN CREATED COURSE
				// ---------------------------------------------

				return CreatedAtAction(
					nameof(GetCourse),
					new
					{
						id = createdCourse.courseid
					},
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


		// =====================================================
		// UPDATE COURSE
		// =====================================================

		[HttpPut("{id}")]
		public async Task<IActionResult> PutCourse(
			int id,
			[FromBody] CourseRequestDto courseDto)
		{
			try
			{
				// ---------------------------------------------
				// VALIDATION
				// ---------------------------------------------

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}


				// ---------------------------------------------
				// DATE VALIDATION
				// ---------------------------------------------

				if (!courseDto.StartDate.HasValue)
				{
					return BadRequest(new
					{
						Message = "Start Date is required."
					});
				}

				if (!courseDto.EndDate.HasValue)
				{
					return BadRequest(new
					{
						Message = "End Date is required."
					});
				}


				if (courseDto.EndDate.Value < courseDto.StartDate.Value)
				{
					return BadRequest(new
					{
						Message = "End Date cannot be before Start Date."
					});
				}


				// ---------------------------------------------
				// FIND EXISTING COURSE
				// ---------------------------------------------

				var existingCourse =
					await _courseRepository.GetCourseById(id);

				if (existingCourse == null)
				{
					return NotFound(new
					{
						Message = "Course not found."
					});
				}


				// ---------------------------------------------
				// UPDATE COURSE
				// ---------------------------------------------

				existingCourse.coursename =
					courseDto.CourseName;

				existingCourse.description =
					courseDto.Description;

				existingCourse.stdid =
					courseDto.StdId;

				existingCourse.userid =
					courseDto.UserId;

				existingCourse.start_date =
					courseDto.StartDate.Value;

				existingCourse.end_date =
					courseDto.EndDate.Value;

				existingCourse.fees =
					courseDto.Fees;

				existingCourse.status =
					string.IsNullOrWhiteSpace(courseDto.Status)
						? "Active"
						: courseDto.Status;

				existingCourse.modified_by =
					"Admin";

				existingCourse.modified_at =
					DateTime.Now;

				existingCourse.is_active = courseDto.IsActive;


				// ---------------------------------------------
				// SAVE UPDATE
				// ---------------------------------------------

				await _courseRepository.UpdateCourse(
					existingCourse
				);


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


		// =====================================================
		// DELETE COURSE
		// =====================================================

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			try
			{
				// ---------------------------------------------
				// FIND COURSE
				// ---------------------------------------------

				var course =
					await _courseRepository.GetCourseById(id);

				if (course == null)
				{
					return NotFound(new
					{
						Message = "Course not found."
					});
				}


				// ---------------------------------------------
				// DELETE
				// ---------------------------------------------

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