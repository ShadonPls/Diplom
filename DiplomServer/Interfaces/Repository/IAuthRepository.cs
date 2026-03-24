using DiplomServer.Models;
using DiplomServer.Models.DTO.Auth;

namespace DiplomServer.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ValidatePasswordAsync(User user, string password);
        Task<User> CreateAsync(RegisterDto dto);
        Task<bool> EmailExistsAsync(string email);
    }

}
