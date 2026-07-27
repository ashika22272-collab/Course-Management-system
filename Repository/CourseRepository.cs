using Dapper;
using Microsoft.Data.SqlClient;
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


		public async Task<IEnumerable<Course>> GetAllCourses()
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = "SELECT * FROM Courses";

			return await connection.QueryAsync<Course>(sql);
		}


		public async Task<Course?> GetCourseById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = "SELECT * FROM Courses WHERE courseid = @Id";

			return await connection.QueryFirstOrDefaultAsync<Course>(
				sql,
				new { Id = id });
		}


		public async Task<Course> AddCourse(Course course)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = @"INSERT INTO Courses
                        (coursename, description, stdid, userid,
                         start_date, end_date, fees, status,
                         modified_by, modified_at,
                         created_by, created_at, is_active)

                       VALUES

                        (@coursename, @description, @stdid, @userid,
                         @start_date, @end_date, @fees, @status,
                         @modified_by, @modified_at,
                         @created_by, @created_at, @is_active);

                       SELECT CAST(SCOPE_IDENTITY() AS INT);";


			int id = await connection.ExecuteScalarAsync<int>(
				sql,
				course);


			course.courseid = id;

			return course;
		}


		public async Task UpdateCourse(Course course)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));


			string sql = @"UPDATE Courses
                       SET coursename=@coursename,
                           description=@description,
                           stdid=@stdid,
                           userid=@userid,
                           start_date=@start_date,
                           end_date=@end_date,
                           fees=@fees,
                           status=@status,
                           modified_by=@modified_by,
                           modified_at=@modified_at,
                           created_by=@created_by,
                           created_at=@created_at,
                           is_active=@is_active
                       WHERE courseid=@courseid";


			await connection.ExecuteAsync(sql, course);
		}


		public async Task DeleteCourse(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				string sql = @"
                    DELETE FROM Assignment
                    WHERE courseid = @Id;

                    DELETE FROM Courses
                    WHERE courseid = @Id;
                ";

				await connection.ExecuteAsync(
					sql,
					new { Id = id },
					transaction);

				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}
	}
}