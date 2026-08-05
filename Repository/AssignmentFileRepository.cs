using Course_Management.Interface;
using Course_Management.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Course_Management.Repository
{
	public class AssignmentFileRepository : IAssignmentFileRepository
	{
		private readonly IConfiguration _configuration;

		public AssignmentFileRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		private IDbConnection Connection =>
			new SqlConnection(_configuration.GetConnectionString("CollegeDB"));

		public async Task<int> UploadAssignment(AssignmentFile assignmentFile)
		{
			using var db = Connection;

			string sql = @"
                INSERT INTO AssignmentFiles
                (
                    AssignmentName,
                    FileName,
                    FilePath,
                    UploadedDate
                )
                VALUES
                (
                    @AssignmentName,
                    @FileName,
                    @FilePath,
                    @UploadedDate
                )";

			return await db.ExecuteAsync(sql, assignmentFile);
		}

		public async Task<IEnumerable<AssignmentFile>> GetAssignments()
		{
			using var db = Connection;

			string sql = @"SELECT *
                           FROM AssignmentFiles
                           ORDER BY UploadedDate DESC";

			return await db.QueryAsync<AssignmentFile>(sql);
		}

		public async Task<AssignmentFile?> GetAssignmentById(int id)
		{
			using var db = Connection;

			string sql = @"SELECT *
                           FROM AssignmentFiles
                           WHERE Id = @Id";

			return await db.QueryFirstOrDefaultAsync<AssignmentFile>(
				sql,
				new { Id = id });
		}

		public async Task DeleteAssignment(int id)
		{
			using var db = Connection;

			string sql = @"DELETE
                           FROM AssignmentFiles
                           WHERE Id = @Id";

			await db.ExecuteAsync(
				sql,
				new { Id = id });
		}
	}
}