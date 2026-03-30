using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Repositories.Interfaces;

namespace DiplomServer.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly ICurrentUserService _currentUserService;

        public LookupService(
            ILookupRepository lookupRepository,
            ICurrentUserService currentUserService)
        {
            _lookupRepository = lookupRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<SelectListItemDto>> GetTeacherDisciplinesAsync()
        {
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");

            var disciplines = await _lookupRepository.GetTeacherDisciplinesAsync(_currentUserService.TeacherId.Value);

            return disciplines.Select(d => new SelectListItemDto
            {
                Value = d.Id,
                Text = d.Name
            }).ToList();
        }

        public async Task<List<SelectListItemDto>> GetAttestTypesAsync()
        {
            var attestTypes = await _lookupRepository.GetAttestTypesAsync();

            return attestTypes.Select(a => new SelectListItemDto
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
    }
}