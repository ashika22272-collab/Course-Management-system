using Dapper;
using Microsoft.Data.SqlClient;
using Course_Management.Interface;
using Course_Management.Models;
using System.Data;

namespace Course_Management.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly IConfiguration _configuration;

		public UserRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// GET ALL USERS
		public async Task<IEnumerable<User>> GetAllUsers()
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryAsync<User>(
				"GetAllUsers",
				commandType: CommandType.StoredProcedure);
		}

		// GET USER BY ID
		public async Task<User?> GetUserById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryFirstOrDefaultAsync<User>(
				"GetUserById",
				new
				{
					Id = id
				},
				commandType: CommandType.StoredProcedure);
		}

		// ADD USER
		public async Task AddUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.ExecuteAsync(
				"AddUser",
				new
				{
					user.firstname,
					user.lastname,
					user.email,
					user.phoneno,
					user.age,
					user.departmentid,
					user.role,
					user.password,
					user.registered_date,
					user.status,
					user.created_by,
					user.created_at,
					user.modified_by,
					user.modified_at,
					user.is_active
				},
				commandType: CommandType.StoredProcedure);
		}

		// UPDATE USER
		public async Task UpdateUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.ExecuteAsync(
				"UpdateUser",
				new
				{
					user.userid,
					user.firstname,
					user.lastname,
					user.email,
					user.phoneno,
					user.age,
					user.departmentid,
					user.role,
					user.password,
					user.registered_date,
					user.status,
					user.modified_by,
					user.modified_at,
					user.created_by,
					user.created_at,
					user.is_active
				},
				commandType: CommandType.StoredProcedure);
		}

		// DELETE USER
		public async Task DeleteUser(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.ExecuteAsync(
				"DeleteUser",
				new
				{
					Id = id
				},
				commandType: CommandType.StoredProcedure);
		}

		// SEARCH + FILTER + SORT + PAGINATION
		public async Task<IEnumerable<User>> SearchUsers(UserSearchRequest request)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string query = @"SELECT *
                             FROM Users
                             WHERE 1 = 1";

			if (!string.IsNullOrEmpty(request.Search))
			{
				query += @" AND (
                                firstname LIKE @Search
                                OR lastname LIKE @Search
                                OR email LIKE @Search
                                OR phoneno LIKE @Search
                                OR role LIKE @Search
                                OR status LIKE @Search
                            )";
			}

			if (request.DepartmentId.HasValue)
			{
				query += " AND departmentid = @DepartmentId";
			}

			if (!string.IsNullOrEmpty(request.Role))
			{
				query += " AND role = @Role";
			}

			if (!string.IsNullOrEmpty(request.Status))
			{
				query += " AND status = @Status";
			}

			switch (request.SortBy?.ToLower())
			{
				case "firstname":
					query += " ORDER BY firstname";
					break;

				case "email":
					query += " ORDER BY email";
					break;

				case "phoneno":
					query += " ORDER BY phoneno";
					break;

				case "registered_date":
					query += " ORDER BY registered_date";
					break;

				default:
					query += " ORDER BY userid";
					break;
			}

			query += request.SortOrder?.ToLower() == "desc"
				? " DESC"
				: " ASC";

			query += @" OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY";

			var parameters = new
			{
				Search = "%" + (request.Search ?? "") + "%",
				request.DepartmentId,
				request.Role,
				request.Status,
				Offset = (request.Page - 1) * request.PageSize,
				request.PageSize
			};

			return await connection.QueryAsync<User>(query, parameters);
		}

		// LOGIN
		public async Task<User?> Login(string email, string password)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			return await connection.QueryFirstOrDefaultAsync<User>(
				"UserLogin",
				new
				{
					Email = email,
					Password = password
				},
				commandType: CommandType.StoredProcedure);
		}
	}
}