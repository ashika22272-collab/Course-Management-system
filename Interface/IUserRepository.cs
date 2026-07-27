using Course_Management.Models;

namespace Course_Management.Interface
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllUsers();
		Task<User?> GetUserById(int id);
		Task<User> AddUser(User user);
		Task UpdateUser(User user);
		Task DeleteUser(int id);
	}
}