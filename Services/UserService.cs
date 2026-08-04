using Course_Management.Interface;
using Course_Management.Models;

namespace Course_Management.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;

		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<IEnumerable<User>> GetAllUsers()
		{
			return await _userRepository.GetAllUsers();
		}

		public async Task<User?> GetUserById(int id)
		{
			return await _userRepository.GetUserById(id);
		}

		public async Task AddUser(User user)
		{
			await _userRepository.AddUser(user);
		}

		public async Task UpdateUser(User user)
		{
			await _userRepository.UpdateUser(user);
		}

		public async Task DeleteUser(int id)
		{
			await _userRepository.DeleteUser(id);
		}

		public async Task<User?> Login(string email, string password)
		{
			return await _userRepository.Login(email, password);
		}

		public async Task<IEnumerable<User>> SearchUsers(UserSearchRequest request)
		{
			return await _userRepository.SearchUsers(request);
		}
	}
}