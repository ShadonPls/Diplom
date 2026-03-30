using System.Security.Claims;
using DiplomServer.Application.Interfaces;

namespace DiplomServer.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public uint UserId => ParseUInt(ClaimTypes.NameIdentifier);

        public uint? TeacherId
        {
            get
            {
                var value = User?.FindFirst("TeacherId")?.Value;
                if (uint.TryParse(value, out var result))
                    return result;

                return null;
            }
        }

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

        private uint ParseUInt(string claimType)
        {
            var value = User?.FindFirst(claimType)?.Value;

            if (!uint.TryParse(value, out var result))
                throw new UnauthorizedAccessException($"Claim '{claimType}' отсутствует или некорректен.");

            return result;
        }
    }
}