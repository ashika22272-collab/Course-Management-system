using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface ICourseRepository
	{
		Task<IEnumerable<Course>> GetAllCourses();
		Task<Course?> GetCourseById(int id);
		Task<Course> AddCourse(Course course);
		Task UpdateCourse(Course course);
		Task DeleteCourse(int id);
	}
}