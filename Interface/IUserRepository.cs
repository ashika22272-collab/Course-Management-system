using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface IUserRepository
	{
		// GET ALL USERS
		Task<IEnumerable<User>> GetAllUsers();

		// GET USER BY ID
		Task<User?> GetUserById(int id);

		// GET USER BY EMAIL
		Task<User?> GetUserByEmail(string email);

		// ADD USER
		Task AddUser(User user);

		// UPDATE USER
		Task UpdateUser(User user);

		// DELETE USER
		Task DeleteUser(int id);

		// LOGIN
		Task<User?> Login(string email, string password);

		// SEARCH USERS
		Task<IEnumerable<User>> SearchUsers(UserSearchRequest request);
	}
}