using DiplomServer.Data;
using DiplomServer.Interfaces.Repository;
using DiplomServer.Models;
using DiplomServer.Models.DTO.Auth;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Repositories;
public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ValidatePasswordAsync(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<User> CreateAsync(RegisterDto dto)
    {
        if (await EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email уже зарегистрирован");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
