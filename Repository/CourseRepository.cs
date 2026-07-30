using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Repository
{
	public class CourseRepository : ICourseRepository
	{
		private readonly IConfiguration _configuration;

		public CourseRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		// GET ALL COURSES
		public async Task<IEnumerable<Course>> GetAllCourses()
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryAsync<Course>(
				"GetAllCourses",
				commandType: CommandType.StoredProcedure);
		}


		// GET COURSE BY ID
		public async Task<Course?> GetCourseById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryFirstOrDefaultAsync<Course>(
				"GetCourseByID",
				new { CourseID = id },
				commandType: CommandType.StoredProcedure);
		}


		// ADD COURSE
		public async Task<Course> AddCourse(Course course)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			var parameters = new
			{
				course.coursename,
				course.description,
				course.stdid,
				course.userid,
				course.start_date,
				course.end_date,
				course.fees,
				course.status,
				course.modified_by,
				course.modified_at,
				course.created_by,
				course.created_at,
				course.is_active
			};

			int id = await connection.ExecuteScalarAsync<int>(
				"AddCourse",
				parameters,
				commandType: CommandType.StoredProcedure);

			course.courseid = id;

			return course;
		}


		// UPDATE COURSE
		public async Task UpdateCourse(Course course)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.ExecuteAsync(
				"UpdateCourse",
				course,
				commandType: CommandType.StoredProcedure);
		}


		// DELETE COURSE
		public async Task DeleteCourse(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.ExecuteAsync(
				"DeleteCourse",
				new { Id = id },
				commandType: CommandType.StoredProcedure);
		}
	}
}