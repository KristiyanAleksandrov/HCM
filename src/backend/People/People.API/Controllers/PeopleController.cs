using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using People.Application.Common;
using People.Application.Interfaces;
using People.Application.RequestModels;
using People.Application.ResponseModels;

namespace People.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IPeopleService personService;

        public PeopleController(IPeopleService personService)
        {
            this.personService = personService;
        }

        [HttpPost]
        //[Authorize(Roles = RoleNames.HRAdmin)]
        public async Task<IActionResult> Add(CreatePersonRequest req, CancellationToken ct)
        {
            var id = await personService.AddAsync(req, ct);

            return Ok(id);
        }

        [HttpPut]
        //[Authorize(Roles = $"{RoleNames.HRAdmin},{RoleNames.Manager}")]
        public async Task Update(Guid id, UpdatePersonRequest req, CancellationToken ct)
        {
            await personService.UpdateAsync(id, req, ct);
        }

        [HttpDelete]
        //[Authorize(Roles = RoleNames.HRAdmin)]
        public async Task Delete(Guid id, CancellationToken ct)
        {
            await personService.DeleteAsync(id, ct);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = $"{RoleNames.HRAdmin},{RoleNames.Manager},{RoleNames.Employee}")]
        public async Task<ActionResult<PersonResponse>> Get(Guid id, CancellationToken ct)
        {
            var person = await personService.GetByIdAsync(id, ct);

            return Ok(person);
        }

        [HttpGet]
        //[Authorize(Roles = $"{RoleNames.HRAdmin},{RoleNames.Manager}")]
        public async Task<ActionResult<IEnumerable<PersonResponse>>> GetAll(CancellationToken ct)
        {
            return Ok(await personService.GetAllAsync(ct));
        }
    }
}
