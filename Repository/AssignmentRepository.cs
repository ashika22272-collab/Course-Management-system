using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
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

		// GET ALL ASSIGNMENTS
		public async Task<IEnumerable<Assignment>> GetAllAssignments()
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryAsync<Assignment>(
				"GetAllAssignments",
				commandType: CommandType.StoredProcedure);
		}

		// GET ASSIGNMENT BY ID
		public async Task<Assignment?> GetAssignmentById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryFirstOrDefaultAsync<Assignment>(
				"GetAssignmentById",
				new { AssignmentID = id },
				commandType: CommandType.StoredProcedure);
		}

		// ADD ASSIGNMENT
		public async Task<int> AddAssignment(Assignment assignment)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			var parameters = new
			{
				userid = assignment.userid,
				courseid = assignment.courseid,
				assignmenttitle = assignment.assignmenttitle,
				modifiedby = assignment.modifiedby,
				modifiedat = assignment.modifiedat,
				createdby = assignment.createdby,
				createdat = assignment.createdat,
				isactive = assignment.isactive
			};

			return await connection.ExecuteScalarAsync<int>(
				"AddAssignment",
				parameters,
				commandType: CommandType.StoredProcedure);
		}

		// UPDATE ASSIGNMENT
		public async Task<int> UpdateAssignment(Assignment assignment)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			var parameters = new
			{
				assignmentid = assignment.assignmentid,
				assignmenttitle = assignment.assignmenttitle,
				modifiedby = assignment.modifiedby,
				modifiedat = assignment.modifiedat,
				isactive = assignment.isactive
			};

			return await connection.ExecuteAsync(
				"UpdateAssignment",
				parameters,
				commandType: CommandType.StoredProcedure);
		}

		// DELETE ASSIGNMENT
		public async Task<int> DeleteAssignment(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.ExecuteAsync(
				"DeleteAssignment",
				new { AssignmentID = id },
				commandType: CommandType.StoredProcedure);
		}
	}
}