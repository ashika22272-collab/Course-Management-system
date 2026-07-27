using Dapper;
using Microsoft.Data.SqlClient;
using Course_Management.Interface;
using Course_Management.Models;

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

			string sql = "SELECT * FROM Users";

			return await connection.QueryAsync<User>(sql);
		}


		// GET USER BY ID
		public async Task<User?> GetUserById(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = "SELECT * FROM Users WHERE userid = @Id";

			return await connection.QueryFirstOrDefaultAsync<User>(
				sql,
				new { Id = id });
		}


		// ADD USER
		public async Task<User> AddUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = @"
                INSERT INTO Users
                (
                    firstname,
                    lastname,
                    email,
                    phoneno,
                    age,
                    departmentid,
                    role,
                    password,
                    registered_date,
                    status,
                    modified_by,
                    modified_at,
                    created_by,
                    created_at,
                    is_active
                )
                VALUES
                (
                    @firstname,
                    @lastname,
                    @email,
                    @phoneno,
                    @age,
                    @departmentid,
                    @role,
                    @password,
                    @registered_date,
                    @status,
                    @modified_by,
                    @modified_at,
                    @created_by,
                    @created_at,
                    @is_active
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

			int id = await connection.ExecuteScalarAsync<int>(
				sql,
				user);

			user.userid = id;

			return user;
		}


		// UPDATE USER
		public async Task UpdateUser(User user)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			string sql = @"
                UPDATE Users
                SET
                    firstname = @firstname,
                    lastname = @lastname,
                    email = @email,
                    phoneno = @phoneno,
                    age = @age,
                    departmentid = @departmentid,
                    role = @role,
                    password = @password,
                    registered_date = @registered_date,
                    status = @status,
                    modified_by = @modified_by,
                    modified_at = @modified_at,
                    created_by = @created_by,
                    created_at = @created_at,
                    is_active = @is_active

                WHERE userid = @userid
            ";

			await connection.ExecuteAsync(sql, user);
		}


		// DELETE USER
		public async Task DeleteUser(int id)
		{
			using var connection = new SqlConnection(
				_configuration.GetConnectionString("CollegeDB"));

			await connection.OpenAsync();

			string sql = @"
                DELETE FROM Assignment
                WHERE UserID = @Id;

                DELETE FROM Enrollment
                WHERE UserID = @Id;

                DELETE FROM Payments
                WHERE UserID = @Id;

                DELETE FROM Submission
                WHERE UserID = @Id;

                DELETE FROM Users
                WHERE UserID = @Id;
            ";

			await connection.ExecuteAsync(
				sql,
				new { Id = id });
		}
	}
}