using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Services
{
	public class UserService
	{
		private readonly IUserRepository _repository;

		public UserService(IUserRepository repository)
		{
			_repository = repository;
		}

		// Get all users
		public async Task<IEnumerable<User>> GetUsers()
		{
			return await _repository.GetAllUsers();
		}

		// Get user by ID
		public async Task<User?> GetUser(int id)
		{
			return await _repository.GetUserById(id);
		}

		// Create user
		public async Task<User> CreateUser(User user)
		{
			await _repository.AddUser(user);
			return user;
		}

		// Update user
		public async Task UpdateUser(User user)
		{
			await _repository.UpdateUser(user);
		}

		// Delete user
		public async Task DeleteUser(int id)
		{
			await _repository.DeleteUser(id);
		}

		// Search, Filter, Sort and Pagination
		public async Task<IEnumerable<User>> SearchUsers(UserSearchRequest request)
		{
			return await _repository.SearchUsers(request);
		}
	}
}