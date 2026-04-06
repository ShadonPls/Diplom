using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Domain.Enums;
using DiplomServer.Infrastructure.Auth;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Repositories.Interfaces;

namespace DiplomServer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IAuthRepository authRepository,
            ICurrentUserService currentUserService,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _authRepository = authRepository;
            _currentUserService = currentUserService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Логин и пароль обязательны.");

            var user = await _authRepository.GetByLoginAsync(dto.Login.Trim().ToLower());
            if (user is null || !await _authRepository.ValidatePasswordAsync(user, dto.Password))
                throw new UnauthorizedAccessException("Неверный логин или пароль.");

            return BuildAuthResponse(user);
        }


        public async Task<CurrentUserDto> GetCurrentUserAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("Пользователь не авторизован.");

            var user = await _authRepository.GetByIdAsync(_currentUserService.UserId);
            if (user is null)
                throw new UnauthorizedAccessException("Пользователь не найден.");

            return new CurrentUserDto
            {
                Id = (uint)user.Id,
                Login = user.Login,
                TeacherId = user.IdUser
            };
        }

        private AuthResponseDto BuildAuthResponse(ScheduleUser user)
        {
            return new AuthResponseDto
            {
                Token = _jwtTokenGenerator.Generate(user),
                User = new CurrentUserDto
                {
                    Id = (uint)user.Id,
                    Login = user.Login,
                    TeacherId = user.IdUser
                }
            };
        }
    }
}