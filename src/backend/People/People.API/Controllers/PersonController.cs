using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using People.Application.Interfaces;
using People.Application.RequestModels;

namespace People.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        //[Authorize(Roles = "Manager")]
        //[HttpGet("test")]
        //public IActionResult PeopleSecure()
        //{
        //    var username = User.Identity?.Name;
        //    return Ok($"People data secured for {username}");
        //}
        private readonly IPersonService personService;

        public PersonController(IPersonService personService)
        {
            this.personService = personService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreatePersonRequest req, CancellationToken ct)
        {
            var id = await personService.AddAsync(req, ct);

            return Ok(id);
        }

        [HttpPut]
        public async Task Update(Guid id, UpdatePersonRequest req, CancellationToken ct)
        {
            await personService.UpdateAsync(id, req, ct);
        }

        [HttpDelete]
        public async Task Delete(Guid id, CancellationToken ct)
        {
            await personService.DeleteAsync(id, ct);
        }
    }
}
