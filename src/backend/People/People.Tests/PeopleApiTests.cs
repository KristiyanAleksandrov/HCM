using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using People.Application.RequestModels;
using People.Application.ResponseModels;
using People.Infrastructure.Data;
using System.Net;
using System.Text;
using Xunit;

namespace People.Tests
{
    public class PeopleApiTests : IClassFixture<PeopleApiFactory>
    {
        private readonly HttpClient client;
        private readonly PeopleApiFactory factory;

        public PeopleApiTests(PeopleApiFactory factory)
        {
            this.factory = factory;
            client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
            db.Database.EnsureDeleted();
            db.SaveChanges();
        }

        private static StringContent ToJson(object obj) =>
            new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        private async Task<Guid> CreatePersonAsync()
        {
            var request = new CreatePersonRequest
            {
                FirstName = "Kristiyan",
                LastName = "Aleksandrov",
                Email = Guid.NewGuid() + "@gmail.com",
                Position = "QA"
            };

            var response = await client.PostAsync("/people", ToJson(request));
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Guid>(body);
        }

        [Fact]
        public async Task AddPerson_ReturnsBadRequest_WhenInvalidPayload()
        {
            var response = await client.PostAsync("/people",
             new StringContent("{}", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddPerson_ReturnsConflict_WhenEmailExists()
        {
            var email = "kris@gmail.com";

            var req = new CreatePersonRequest
            {
                FirstName = "Kristiyan",
                LastName = "Aleksandrov",
                Email = email,
                Position = "QA"
            };

            var first = await client.PostAsync("/people", ToJson(req));
            first.StatusCode.Should().Be(HttpStatusCode.OK);

            var second = await client.PostAsync("/people", ToJson(req));
            second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task GetPerson_ReturnsOk_WhenExists()
        {
            var id = await CreatePersonAsync();

            var response = await client.GetAsync($"/people/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            var person = JsonConvert.DeserializeObject<PersonResponse>(body);
            person.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetPerson_ReturnsNotFound_WhenMissing()
        {
            var response = await client.GetAsync($"/people/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsOK()
        {
            var id = await CreatePersonAsync();

            var update = new UpdatePersonRequest { FirstName = "Updated" };

            var resp = await client.PutAsync($"/people?id={id}", ToJson(update));
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var verify = await client.GetAsync($"/people/{id}");
            var body = await verify.Content.ReadAsStringAsync();
            var person = JsonConvert.DeserializeObject<PersonResponse>(body);
            person.FirstName.Should().Be("Updated");
        }

        [Fact]
        public async Task DeletePerson_ReturnsOK()
        {
            var id = await CreatePersonAsync();

            var del = await client.DeleteAsync($"/people?id={id}");
            del.StatusCode.Should().Be(HttpStatusCode.OK);

            var verify = await client.GetAsync($"/people/{id}");
            verify.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ReturnsEmpty_WhenNoPeople()
        {
            var response = await client.GetAsync("/people");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<PersonResponse>>(json);
            list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsList_WhenPeopleExist()
        {
            await CreatePersonAsync();
            await CreatePersonAsync();

            var response = await client.GetAsync("/people");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<PersonResponse>>(json);
            list.Should().HaveCount(2);
        }
    }
}
