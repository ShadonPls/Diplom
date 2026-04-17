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


        [HttpPost("retake-directions/pdf")]
        public async Task<IActionResult> GetRetakeDirectionsPdf([FromBody] IList<uint> ids)
        {
            var fileBytes = await _pdfService.GenerateRetakeDirectionPdfAsync(ids);

            return File(
                fileBytes,
                "application/pdf",
                "retake-directions.pdf");
        }
    }
}