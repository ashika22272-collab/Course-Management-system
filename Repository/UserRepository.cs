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
				_configuration.GetConnectionString("CollegeDB")
			);


			return await connection.QueryAsync<User>(
				"GetAllUsers",
				commandType: CommandType.StoredProcedure
			);
		}



		// GET USER BY ID
		public async Task<User?> GetUserById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);


			return await connection.QueryFirstOrDefaultAsync<User>(
				"GetUserById",
				new
				{
					userid = id
				},
				commandType: CommandType.StoredProcedure
			);
		}




		// ADD USER
		public async Task AddUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);


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
					user.registered_date
				},
				commandType: CommandType.StoredProcedure
			);
		}




		// UPDATE USER
		public async Task UpdateUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);


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
					user.password
				},
				commandType: CommandType.StoredProcedure
			);
		}




		// DELETE USER
		public async Task DeleteUser(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);


			await connection.ExecuteAsync(
				"DeleteUser",
				new
				{
					userid = id
				},
				commandType: CommandType.StoredProcedure
			);
		}




		// SEARCH + FILTER + SORT + PAGINATION
		public async Task<IEnumerable<User>> SearchUsers(UserSearchRequest request)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB")
			);


			string query = @"SELECT *
                             FROM Users
                             WHERE 1=1";



			// Search by User Name
			if (!string.IsNullOrEmpty(request.SearchName))
			{
				query += @" AND 
                (firstname LIKE @SearchName 
                OR lastname LIKE @SearchName)";
			}



			// Filter Department
			if (request.DepartmentId.HasValue)
			{
				query += " AND departmentid=@DepartmentId";
			}



			// Filter Role
			if (!string.IsNullOrEmpty(request.Role))
			{
				query += " AND role=@Role";
			}



			// Filter Status
			if (!string.IsNullOrEmpty(request.Status))
			{
				query += " AND status=@Status";
			}




			// Sorting
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



			if (request.SortOrder?.ToLower() == "desc")
			{
				query += " DESC";
			}
			else
			{
				query += " ASC";
			}




			// Pagination
			query += @" OFFSET @Offset ROWS
                         FETCH NEXT @PageSize ROWS ONLY";




			var parameters = new
			{
				SearchName = "%" + request.SearchName + "%",

				request.DepartmentId,

				request.Role,

				request.Status,


				Offset = (request.Page - 1) * request.PageSize,

				request.PageSize
			};



			return await connection.QueryAsync<User>(
				query,
				parameters
			);
		}

	}
}