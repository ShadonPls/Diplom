using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.RetakeDirections;
using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Documents;
using DiplomServer.Infrastructure.Repositories;
using DiplomServer.Infrastructure.Repositories.Interfaces;

namespace DiplomServer.Application.Services
{
    public class PdfService : IPdfService
    {
        private readonly IRetakeDirectionService _retakeDirectionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly PdfGenerator _pdfGenerator;
        private readonly IAuthService _authService;
        public PdfService(
            IRetakeDirectionService retakeDirectionService,
            ICurrentUserService currentUserService,
            PdfGenerator pdfGenerator,
            IAuthService authService)
        {
            _retakeDirectionService = retakeDirectionService;
            _currentUserService = currentUserService;
            _pdfGenerator = pdfGenerator;
            _authService = authService;
        }


        public async Task<byte[]> GenerateRetakeDirectionPdfAsync(IList<uint> retakeDirectionIds)
        {
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");

            var teacher = await _authService.GetCurrentUserAsync();

            var items = new List<(RetakeDirectionDetailsDto, CurrentUserDto)>();
            foreach (var id in retakeDirectionIds)
            {
                var direction = await _retakeDirectionService.GetByIdAsync(id);
                items.Add((direction, teacher));
            }

            return _pdfGenerator.GenerateRetakeDirectionPdf(items);
        }
    }
}