using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/lookup")]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("disciplines")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetTeacherDisciplines()
        {
            return Ok(await _lookupService.GetTeacherDisciplinesAsync());
        }

        [HttpGet("attest-types")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetAttestTypes()
        {
            return Ok(await _lookupService.GetAttestTypesAsync());
        }
    }
}