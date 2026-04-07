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

        public async Task<byte[]> GenerateRetakeDirectionPdfAsync(uint retakeDirectionId)
        {
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");

            var teacherId = _currentUserService.TeacherId.Value;

            var direction = await _retakeDirectionService.GetByIdAsync(retakeDirectionId);
            var teacher = await _authService.GetCurrentUserAsync();
            return _pdfGenerator.GenerateRetakeDirectionPdf(direction, teacher);
        }
    }
}