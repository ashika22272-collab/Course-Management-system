using Microsoft.AspNetCore.Mvc;
using Course_Management.Models;
using Course_Management.Interface;

namespace Course_Management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseRepository _courseRepository;

		public CourseController(ICourseRepository courseRepository)
		{
			_courseRepository = courseRepository;
		}

		/// <summary>
		/// Retrieves all courses.
		/// </summary>
		/// <returns>Returns a list of all courses.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
		{
			return Ok(await _courseRepository.GetAllCourses());
		}

		/// <summary>
		/// Retrieves a course by its ID.
		/// </summary>
		/// <param name="id">The ID of the course.</param>
		/// <returns>Returns the course if found; otherwise, returns NotFound.</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<Course>> GetCourse(int id)
		{
			var course = await _courseRepository.GetCourseById(id);

			if (course == null)
			{
				return NotFound();
			}

			return Ok(course);
		}

		/// <summary>
		/// Creates a new course.
		/// </summary>
		/// <param name="course">The course details.</param>
		/// <returns>Returns the newly created course.</returns>
		[HttpPost]
		public async Task<ActionResult<Course>> PostCourse(Course course)
		{
			var createdCourse = await _courseRepository.AddCourse(course);

			return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.courseid }, createdCourse);
		}

		/// <summary>
		/// Updates an existing course.
		/// </summary>
		/// <param name="id">The ID of the course to update.</param>
		/// <param name="course">The updated course details.</param>
		/// <returns>Returns NoContent if the update is successful.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCourse(int id, Course course)
		{
			var existingCourse = await _courseRepository.GetCourseById(id);

			if (existingCourse == null)
			{
				return NotFound();
			}

			course.courseid = id;

			await _courseRepository.UpdateCourse(course);

			return NoContent();
		}

		/// <summary>
		/// Deletes a course by its ID.
		/// </summary>
		/// <param name="id">The ID of the course to delete.</param>
		/// <returns>Returns NoContent if the deletion is successful.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			var course = await _courseRepository.GetCourseById(id);

			if (course == null)
			{
				return NotFound();
			}

			await _courseRepository.DeleteCourse(id);

			return NoContent();
		}
	}
}