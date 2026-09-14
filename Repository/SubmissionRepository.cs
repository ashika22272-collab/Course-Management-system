
using CourseManagementAPI.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CourseManagementAPI.Repositories
{
	public class SubmissionRepository : ISubmissionRepository
	{
		private readonly IConfiguration _configuration;

		public SubmissionRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// Get all submissions made by a particular student
		public async Task<IEnumerable<SubmissionDto>> GetSubmissionsByStudentId(int userId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT
                    s.SubID,
                    s.SubNo,
                    s.SubGrade,
                    s.Feedback,
                    s.UserID,
                    s.CourseID,
                    s.AssignmentID,
                    s.SubmissionStatus,
                    s.FileName,
                    s.FilePath,
                    s.CreatedAt,
                    s.CreatedBy,
                    s.IsActive
                FROM Submission s
                WHERE s.UserID = @UserID
                  AND s.IsActive = 1
                ORDER BY s.CreatedAt DESC;
            ";

			return await connection.QueryAsync<SubmissionDto>(
				sql,
				new { UserID = userId }
			);
		}

		// Get a student's submission for a particular assignment
		public async Task<SubmissionDto?> GetSubmissionByAssignmentAndStudent(
			int assignmentId,
			int userId)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                SELECT TOP 1
                    s.SubID,
                    s.SubNo,
                    s.SubGrade,
                    s.Feedback,
                    s.UserID,
                    s.CourseID,
                    s.AssignmentID,
                    s.SubmissionStatus,
                    s.FileName,
                    s.FilePath,
                    s.CreatedAt,
                    s.CreatedBy,
                    s.IsActive
                FROM Submission s
                WHERE s.AssignmentID = @AssignmentID
                  AND s.UserID = @UserID
                  AND s.IsActive = 1
                ORDER BY s.CreatedAt DESC;
            ";

			return await connection.QueryFirstOrDefaultAsync<SubmissionDto>(
				sql,
				new
				{
					AssignmentID = assignmentId,
					UserID = userId
				}
			);
		}

		// Create a new submission
		public async Task<int> CreateSubmission(SubmissionDto submission)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string getNextIdSql = @"
                SELECT ISNULL(MAX(SubID), 0) + 1
                FROM Submission;
            ";

			int nextSubId = await connection.ExecuteScalarAsync<int>(
				getNextIdSql
			);

			const string sql = @"
                INSERT INTO Submission
                (
                    SubID,
                    SubNo,
                    UserID,
                    CourseID,
                    AssignmentID,
                    SubmissionStatus,
                    FileName,
                    FilePath,
                    CreatedAt,
                    CreatedBy,
                    IsActive
                )
                VALUES
                (
                    @SubID,
                    @SubNo,
                    @UserID,
                    @CourseID,
                    @AssignmentID,
                    @SubmissionStatus,
                    @FileName,
                    @FilePath,
                    @CreatedAt,
                    @CreatedBy,
                    1
                );
            ";

			await connection.ExecuteAsync(
				sql,
				new
				{
					SubID = nextSubId,
					SubNo = nextSubId,
					submission.UserID,
					submission.CourseID,
					submission.AssignmentID,
					SubmissionStatus = "Submitted",
					submission.FileName,
					submission.FilePath,
					CreatedAt = DateTime.Now,
					submission.CreatedBy
				}
			);

			return nextSubId;
		}

		// Update an existing submission
		public async Task<bool> UpdateSubmission(SubmissionDto submission)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);

			const string sql = @"
                UPDATE Submission
                SET
                    FileName = @FileName,
                    FilePath = @FilePath,
                    SubmissionStatus = @SubmissionStatus,
                    ModifiedAt = @ModifiedAt,
                    ModifiedBy = @ModifiedBy
                WHERE SubID = @SubID
                  AND IsActive = 1;
            ";

			int rowsAffected = await connection.ExecuteAsync(
				sql,
				new
				{
					submission.SubID,
					submission.FileName,
					submission.FilePath,
					SubmissionStatus = "Submitted",
					ModifiedAt = DateTime.Now,
					ModifiedBy = submission.CreatedBy
				}
			);

			return rowsAffected > 0;
		}
	}
}

