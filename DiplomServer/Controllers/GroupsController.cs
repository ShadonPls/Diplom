using DiplomServer.Application.DTOs.Common;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomServer.Controllers
{
    [ApiController]
    [Route("api/groups")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly ILookupRepository _lookupRepository;

        public GroupsController(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<SelectListItemDto>>> GetGroups()
        {
            var groups = await _lookupRepository.GetGroupsAsync();

            var result = groups.Select(g => new SelectListItemDto
            {
                Value = g.Id,
                Text = g.Name
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{groupId}/students")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetStudents(uint groupId)
        {
            var students = await _lookupRepository.GetGroupStudentsAsync(groupId);

            var result = students.Select(s => new SelectListItemDto
            {
                Value = s.Id,
                Text = $"{s.LastName} {s.FirstName}".Trim()
            }).ToList();

            return Ok(result);
        }
    }
}