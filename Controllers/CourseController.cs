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


		// GET: api/Course
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
		{
			return Ok(await _courseRepository.GetAllCourses());
		}


		// GET: api/Course/5
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


		// POST: api/Course
		[HttpPost]
		public async Task<ActionResult<Course>> PostCourse(Course course)
		{
			var createdCourse = await _courseRepository.AddCourse(course);

			return CreatedAtAction(
				nameof(GetCourse),
				new { id = createdCourse.courseid },
				createdCourse
			);
		}


		// PUT: api/Course/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCourse(int id, Course course)
		{
			var existingCourse = await _courseRepository.GetCourseById(id);

			if (existingCourse == null)
			{
				return NotFound();
			}

			// Assign URL id to object
			course.courseid = id;

			await _courseRepository.UpdateCourse(course);

			return NoContent();
		}


		// DELETE: api/Course/5
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