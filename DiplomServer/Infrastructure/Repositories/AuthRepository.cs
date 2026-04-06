using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.Common;
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
    public async Task<TypeDto> GetTeacherByIdAsync(uint teacherId)
    {
        var teacher = await _context.Teachers
            .Where(t => t.Id == (int)teacherId && t.Delete == 0)
            .Select(t => new TypeDto
            {
                Id = (int)t.Id,
                Name = $"{t.first_name} {t.middle_name} {t.last_name}"
            })
            .FirstOrDefaultAsync();

        if (teacher is null)
            throw new KeyNotFoundException($"Преподаватель {teacherId} не найден.");

        return teacher;
    }
}