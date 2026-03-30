using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Domain.Enums;
using DiplomServer.Infrastructure.Auth;
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
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Email и пароль обязательны.");

            var user = await _authRepository.GetByEmailAsync(dto.Email.Trim().ToLower());
            if (user is null || !await _authRepository.ValidatePasswordAsync(user, dto.Password))
                throw new UnauthorizedAccessException("Неверный email или пароль.");

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email обязателен.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                throw new ArgumentException("Пароль должен содержать минимум 6 символов.");

            var normalizedEmail = dto.Email.Trim().ToLower();

            if (await _authRepository.EmailExistsAsync(normalizedEmail))
                throw new InvalidOperationException("Email уже зарегистрирован.");

            var user = await _authRepository.CreateAsync(new RegisterRequestDto
            {
                Email = normalizedEmail,
                Password = dto.Password,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? UserRoles.Teacher : dto.Role.Trim()
            });

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
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                TeacherId = user.TeacherId
            };
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                Token = _jwtTokenGenerator.Generate(user),
                User = new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    TeacherId = user.TeacherId
                }
            };
        }
    }
}