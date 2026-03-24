using DiplomServer.Models.DTO.Auth;

namespace DiplomServer.Interfaces.Service
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RegisterAsync(RegisterDto dto); // ← НОВЫЙ!
    }

}
