using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface IUserService
	{
		Task<IEnumerable<User>> GetAllUsers();

		Task<User?> GetUserById(int id);

		Task<User?> GetUserByEmail(string email);

		Task AddUser(User user);

		Task UpdateUser(User user);

		Task DeleteUser(int id);

		Task<IEnumerable<User>> SearchUsers(UserSearchRequest request);

		Task<User?> Login(string email, string password);
	}
}