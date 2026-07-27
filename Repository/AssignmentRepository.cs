using Dapper;
using Microsoft.Data.SqlClient;
using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Repository
{
	public class AssignmentRepository : IAssignmentRepository
	{
		private readonly IConfiguration _configuration;

		public AssignmentRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		public async Task<IEnumerable<Assignment>> GetAllAssignments()
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryAsync<Assignment>(
				"SELECT * FROM Assignment");
		}


		public async Task<Assignment?> GetAssignmentById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryFirstOrDefaultAsync<Assignment>(
				"SELECT * FROM Assignment WHERE assignmentid=@id",
				new { id });
		}


		public async Task<int> AddAssignment(Assignment assignment)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.ExecuteAsync(
				@"INSERT INTO Assignment
                (userid, courseid, assignmenttitle, createdby, createdat, isactive)
                VALUES
                (@userid, @courseid, @assignmenttitle, @createdby, @createdat, @isactive)",
				assignment);
		}


		public async Task<int> UpdateAssignment(Assignment assignment)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.ExecuteAsync(
				@"UPDATE Assignment SET
                assignmenttitle = @assignmenttitle,
                modifiedby = @modifiedby,
                modifiedat = @modifiedat,
                isactive = @isactive
                WHERE assignmentid = @assignmentid",
				assignment);
		}


		public async Task<int> DeleteAssignment(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.ExecuteAsync(
				"DELETE FROM Assignment WHERE assignmentid=@id",
				new { id });
		}
	}
}