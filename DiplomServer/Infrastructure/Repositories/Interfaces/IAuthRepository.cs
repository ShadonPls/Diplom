using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Domain.Entities;

namespace DiplomServer.Infrastructure.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(uint id);
    Task<bool> ValidatePasswordAsync(User user, string password);
    Task<User> CreateAsync(RegisterRequestDto dto);
    Task<bool> EmailExistsAsync(string email);
}