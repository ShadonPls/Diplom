using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Documents;
using DiplomServer.Infrastructure.Repositories.Interfaces;

namespace DiplomServer.Application.Services
{
    public class PdfService : IPdfService
    {
        private readonly IRetakeDirectionRepository _retakeDirectionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly PdfGenerator _pdfGenerator;

        public PdfService(
            IRetakeDirectionRepository retakeDirectionRepository,
            ICurrentUserService currentUserService,
            PdfGenerator pdfGenerator)
        {
            _retakeDirectionRepository = retakeDirectionRepository;
            _currentUserService = currentUserService;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<byte[]> GenerateRetakeDirectionPdfAsync(uint retakeDirectionId)
        {
            if (!_currentUserService.TeacherId.HasValue)
                throw new UnauthorizedAccessException("TeacherId отсутствует.");

            var teacherId = _currentUserService.TeacherId.Value;

            var direction = await _retakeDirectionRepository.GetByIdWithIncludesAsync(retakeDirectionId);

            if (direction is null)
                throw new KeyNotFoundException("Направление не найдено.");

            if (direction.CreatedById != teacherId)
                throw new UnauthorizedAccessException("Нет доступа к данному направлению.");
            throw new UnauthorizedAccessException("asfd");
            //return _pdfGenerator.GenerateRetakeDirectionPdf(direction);
        }
    }
}