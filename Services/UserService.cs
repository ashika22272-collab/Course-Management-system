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

		public async Task<IEnumerable<User>> GetUsers()
		{
			return await _repository.GetAllUsers();
		}

		public async Task<User?> GetUser(int id)
		{
			return await _repository.GetUserById(id);
		}

		public async Task<User> CreateUser(User user)
		{
			return await _repository.AddUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _repository.UpdateUser(user);
		}

		public async Task DeleteUser(int id)
		{
			await _repository.DeleteUser(id);
		}
	}
}
