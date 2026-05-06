using DiplomServer.Application.Interfaces;
using DiplomServer.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/excel")]
    [Authorize]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelParser _excelParser;

        public ExcelController( IExcelParser excelParser)
        {
            _excelParser = excelParser;
        }
        [HttpGet("Excel")]
        public async Task<IActionResult> Excel(uint id)
        {
            try
            {
                var csvData = _excelParser.GetBadGrades(@"C:\Users\user\Downloads\ISP-22-2.xlsx");
                return Ok(csvData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }
    }
}
