using DiplomServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IPdfService _pdfService;

        public DocumentsController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("retake-directions/{id}/pdf")]
        public async Task<IActionResult> GetRetakeDirectionPdf(uint id)
        {
            var fileBytes = await _pdfService.GenerateRetakeDirectionPdfAsync(id);

            return File(
                fileBytes,
                "application/pdf",
                $"retake-direction-{id}.pdf");
        }
    }
}