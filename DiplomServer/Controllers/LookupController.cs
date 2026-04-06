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
        [HttpGet("teachers")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetTeachers()
        {
            return Ok(await _lookupService.GetTeachersAsync());
        }
        [HttpGet("disciplines/{disciplineId}/groups")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetDisciplineGroups(uint disciplineId)
        {
            return Ok(await _lookupService.GetGroupsByDisciplineIdAsync(disciplineId));
        }

        [HttpGet("groups/{groupId}/semestr")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetSemestrGroups(uint groupId)
        {
            return Ok(await _lookupService.GetSemestrByGroupAsync(groupId));
        }
        [HttpGet("groups")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetGroups()
        {
            return Ok(await _lookupService.GetGroupsAsync());
        }

        [HttpGet("groups/{groupId}/students")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetStudents(uint groupId)
        {
            return Ok(await _lookupService.GetGroupStudentsAsync(groupId));
        }
    }
}