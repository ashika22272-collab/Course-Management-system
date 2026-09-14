
using Dapper;
using Microsoft.Data.SqlClient;
using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public class EnrollmentRepository : IEnrollmentRepository
	{
		private readonly IConfiguration _configuration;

		public EnrollmentRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// =========================================================
		// GET STUDENTS BY COURSE ID
		// =========================================================

		public async Task<IEnumerable<EnrollmentStudentDto>> GetStudentsByCourseId(
			int courseId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    u.userid AS UserID,
                    u.firstname AS FirstName,
                    u.lastname AS LastName,
                    u.email AS Email,
                    u.phoneno AS PhoneNo,
                    e.EnrollmentDate,
                    e.Status
                FROM Enrollment e
                INNER JOIN Users u
                    ON e.UserID = u.userid
                WHERE e.CourseID = @CourseID
                  AND e.IsActive = 1
                ORDER BY u.firstname, u.lastname;
            ";

			return await connection.QueryAsync<EnrollmentStudentDto>(
				sql,
				new
				{
					CourseID = courseId
				}
			);
		}


		// =========================================================
		// GET ALL STUDENTS FOR AN INSTRUCTOR
		// =========================================================

		public async Task<IEnumerable<EnrollmentStudentDto>> GetStudentsByInstructorId(
			int instructorId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    u.userid AS UserID,
                    u.firstname AS FirstName,
                    u.lastname AS LastName,
                    u.email AS Email,
                    u.phoneno AS PhoneNo,

                    MAX(e.EnrollmentDate) AS EnrollmentDate,

                    'Enrolled' AS Status

                FROM Enrollment e

                INNER JOIN Users u
                    ON e.UserID = u.userid

                INNER JOIN Courses c
                    ON e.CourseID = c.courseid

                WHERE c.userid = @InstructorID
                  AND e.IsActive = 1

                GROUP BY
                    u.userid,
                    u.firstname,
                    u.lastname,
                    u.email,
                    u.phoneno

                ORDER BY
                    u.firstname,
                    u.lastname;
            ";

			return await connection.QueryAsync<EnrollmentStudentDto>(
				sql,
				new
				{
					InstructorID = instructorId
				}
			);
		}


		// =========================================================
		// GET ENROLLMENTS BY STUDENT ID
		// =========================================================

		public async Task<IEnumerable<StudentEnrollmentDto>> GetEnrollmentsByStudentId(
			int userId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    0 AS EnrollmentId,
                    c.courseid AS CourseId,
                    c.coursename AS CourseName,
                    c.description AS Description,
                    c.category AS Category,

                    ISNULL(
                        u.firstname + ' ' + u.lastname,
                        'Not Assigned'
                    ) AS Instructor,

                    c.duration AS Duration,
                    c.start_date AS StartDate,
                    c.end_date AS EndDate,
                    c.fees AS Fees,

                    e.EnrollmentDate AS EnrollmentDate,
                    e.Status AS Status

                FROM Enrollment e

                INNER JOIN Courses c
                    ON e.CourseID = c.courseid

                LEFT JOIN Users u
                    ON c.userid = u.userid

                WHERE e.UserID = @UserID
                  AND e.IsActive = 1

                ORDER BY
                    e.EnrollmentDate DESC;
            ";

			return await connection.QueryAsync<StudentEnrollmentDto>(
				sql,
				new
				{
					UserID = userId
				}
			);
		}


		// =========================================================
		// ENROLL STUDENT
		// =========================================================

		public async Task<bool> EnrollStudent(
			int courseId,
			int userId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			await connection.OpenAsync();

			using var transaction =
				await connection.BeginTransactionAsync();

			try
			{
				// -------------------------------------------------
				// CHECK COURSE EXISTS
				// -------------------------------------------------

				const string courseCheckSql = @"
                    SELECT COUNT(1)
                    FROM Courses
                    WHERE courseid = @CourseID;
                ";

				var courseExists =
					await connection.ExecuteScalarAsync<int>(
						courseCheckSql,
						new
						{
							CourseID = courseId
						},
						transaction
					);

				if (courseExists == 0)
				{
					await transaction.RollbackAsync();
					return false;
				}


				// -------------------------------------------------
				// CHECK IF STUDENT IS ALREADY ENROLLED
				// -------------------------------------------------

				const string existingEnrollmentSql = @"
                    SELECT COUNT(1)
                    FROM Enrollment
                    WHERE CourseID = @CourseID
                      AND UserID = @UserID
                      AND IsActive = 1;
                ";

				var alreadyEnrolled =
					await connection.ExecuteScalarAsync<int>(
						existingEnrollmentSql,
						new
						{
							CourseID = courseId,
							UserID = userId
						},
						transaction
					);

				if (alreadyEnrolled > 0)
				{
					await transaction.RollbackAsync();
					return false;
				}


				// -------------------------------------------------
				// INSERT ENROLLMENT
				// -------------------------------------------------

				const string insertEnrollmentSql = @"
                    INSERT INTO Enrollment
                    (
                        CourseID,
                        UserID,
                        EnrollmentDate,
                        Status,
                        IsActive
                    )
                    VALUES
                    (
                        @CourseID,
                        @UserID,
                        GETDATE(),
                        'Enrolled',
                        1
                    );
                ";

				await connection.ExecuteAsync(
					insertEnrollmentSql,
					new
					{
						CourseID = courseId,
						UserID = userId
					},
					transaction
				);


				// -------------------------------------------------
				// COMMIT
				// -------------------------------------------------

				await transaction.CommitAsync();

				return true;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}

