using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ScheduleDbContext _context;

    public AuthRepository(ScheduleDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleUser?> GetByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<ScheduleUser?> GetByIdAsync(uint id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ValidatePasswordAsync(ScheduleUser user, string password)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, user.Password));
    }


    public async Task<bool> LoginExistsAsync(string login)
    {
        return await _context.Users.AnyAsync(u => u.Login == login);
    }
}