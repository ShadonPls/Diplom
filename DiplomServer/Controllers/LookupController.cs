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

        [HttpGet("teacher/{disciplineId}")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetTeacherByDisciplines(uint disciplineId)
        {
            return Ok(await _lookupService.GetTeacherByDisciplinesAsync(disciplineId));
        }

        [HttpGet("attest-types")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetAttestTypes()
        {
            return Ok(await _lookupService.GetAttestTypesAsync());
        }
        [HttpGet("teachers/{teacherId}")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetTeachers(uint teacherId)
        {
            return Ok(await _lookupService.GetTeachersByIdAsync(teacherId));
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

        [HttpGet("students/academicDebts")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetStudentsDebts()
        {
            return Ok(await _lookupService.GetStudentsDebts());
        }

        [HttpGet("disciplines/{studentId}/academicDebts")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetDisciplinesStudentDebts(uint studentId)
        {
            return Ok(await _lookupService.GetDisciplinesStudentsDebts(studentId));
        }
        [HttpGet("orders/{number}")]
        public async Task<ActionResult<bool>> TestNumber(uint number)
        {
            return Ok(await _lookupService.TestNumber(number));
        }
    }
}