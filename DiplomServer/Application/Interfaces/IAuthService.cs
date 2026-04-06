using DiplomServer.Application.DTOs.Auth;

namespace DiplomServer.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<CurrentUserDto> GetCurrentUserAsync();
    }
}