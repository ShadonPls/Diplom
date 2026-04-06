using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.Common;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;

namespace DiplomServer.Infrastructure.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<ScheduleUser?> GetByLoginAsync(string login);
    Task<ScheduleUser?> GetByIdAsync(uint id);
    Task<bool> ValidatePasswordAsync(ScheduleUser user, string password);
    Task<bool> LoginExistsAsync(string login);
    Task<TypeDto> GetTeacherByIdAsync(uint teacherId);
}