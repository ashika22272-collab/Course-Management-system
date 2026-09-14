
using Dapper;
using Microsoft.Data.SqlClient;
using CourseManagementAPI.DTOs;

namespace CourseManagementAPI.Repositories
{
	public class PaymentRepository : IPaymentRepository
	{
		private readonly IConfiguration _configuration;

		public PaymentRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		// =====================================================
		// GET PAYMENT HISTORY BY STUDENT ID
		// =====================================================

		public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStudentId(
			int userId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    p.PayID AS PaymentId,

                    c.coursename AS CourseName,

                    p.PaymentDate AS PaymentDate,

                    p.PaymentAmount AS Amount,

                    p.PaymentStatus AS PaymentStatus

                FROM Payments p

                INNER JOIN Courses c
                    ON p.CourseID = c.courseid

                INNER JOIN Enrollment e
                    ON p.CourseID = e.CourseID
                   AND p.UserID = e.UserID

                WHERE p.UserID = @UserID

                  AND p.IsActive = 1

                  AND e.IsActive = 1

                  AND p.PaymentAmount IS NOT NULL

                  AND p.PaymentDate IS NOT NULL

                  AND p.PaymentStatus IS NOT NULL

                ORDER BY p.PaymentDate DESC;
            ";

			return await connection.QueryAsync<PaymentResponseDto>(
				sql,
				new
				{
					UserID = userId
				}
			);
		}


		// =====================================================
		// GET ALL PAYMENTS FOR INSTRUCTOR'S COURSES
		// =====================================================

		public async Task<IEnumerable<InstructorPaymentDto>> GetPaymentsByInstructorId(
			int instructorId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    p.PayID AS PaymentId,

                    u.userid AS UserID,

                    ISNULL(
                        u.firstname + ' ' + u.lastname,
                        'Unknown Student'
                    ) AS StudentName,

                    u.email AS Email,

                    c.coursename AS CourseName,

                    p.PaymentDate AS PaymentDate,

                    p.PaymentAmount AS Amount,

                    p.PaymentStatus AS PaymentStatus

                FROM Payments p

                INNER JOIN Courses c
                    ON p.CourseID = c.courseid

                INNER JOIN Users u
                    ON p.UserID = u.userid

                INNER JOIN Enrollment e
                    ON p.CourseID = e.CourseID
                   AND p.UserID = e.UserID

                WHERE c.userid = @InstructorID

                  AND p.IsActive = 1

                  AND e.IsActive = 1

                  AND p.PaymentAmount IS NOT NULL

                  AND p.PaymentDate IS NOT NULL

                  AND p.PaymentStatus IS NOT NULL

                ORDER BY p.PaymentDate DESC;
            ";

			return await connection.QueryAsync<InstructorPaymentDto>(
				sql,
				new
				{
					InstructorID = instructorId
				}
			);
		}
	}
}

