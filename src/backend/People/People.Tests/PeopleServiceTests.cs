using Moq;
using People.Application.Errors;
using People.Application.Interfaces;
using People.Application.RequestModels;
using People.Application.Services;
using People.Domain.Entities;
using Xunit;

namespace People.Tests
{
    public class PeopleServiceTests
    {
        private readonly Mock<IPeopleRepository> repoMock;
        private readonly PeopleService peopleService;
        private readonly CancellationToken ct = CancellationToken.None;

        public PeopleServiceTests()
        {
            repoMock = new Mock<IPeopleRepository>(MockBehavior.Strict);
            peopleService = new PeopleService(repoMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowBadRequest_WhenFirstNameMissing()
        {
            var req = new CreatePersonRequest
            {
                FirstName = "",
                LastName = "Aleksandrov",
                Email = "kris@gmail.com",
                Position = "Software Engineer"
            };

            await Assert.ThrowsAsync<BadRequestException>(() => peopleService.AddAsync(req, ct));
        }

        [Fact]
        public async Task AddAsync_ShouldThrowConflict_WhenEmailExists()
        {
            var req = new CreatePersonRequest
            {
                FirstName = "Kristiyan",
                LastName = "Aleksandrov",
                Email = "kris@gmail.com",
                Position = "Software Engineer"
            };

            repoMock.Setup(r => r.ExistsAsync(req.Email, ct)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => peopleService.AddAsync(req, ct));
        }

        [Fact]
        public async Task AddAsync_ShouldReturnId_WhenValid()
        {
            var req = new CreatePersonRequest
            {
                FirstName = "Kristiyan",
                LastName = "Aleksandrov",
                Email = "kris@gmail.com",
                Position = "Software Engineer"
            };

            repoMock.Setup(r => r.ExistsAsync(req.Email, ct)).ReturnsAsync(false);
            repoMock.Setup(r => r.AddAsync(It.IsAny<Person>(), ct)).Returns(Task.CompletedTask);
            repoMock.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

            var id = await peopleService.AddAsync(req, ct);

            repoMock.Verify(r => r.AddAsync(It.Is<Person>(p => p.Email == req.Email), ct), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFound_WhenPersonMissing()
        {
            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetAsync(id, ct)).ReturnsAsync((Person?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => peopleService.UpdateAsync(id, new UpdatePersonRequest(), ct));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
        {
            var id = Guid.NewGuid();
            var existing = new Person("Kristiyan", "Aleksandrov", "kris@gmail.com", "Dev") { Id = id };
            var req = new UpdatePersonRequest { Email = "ivan@gmail.com" };

            repoMock.Setup(r => r.GetAsync(id, ct)).ReturnsAsync(existing);
            repoMock.Setup(r => r.ExistsAsync(req.Email, ct)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => peopleService.UpdateAsync(id, req, ct));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFields_WhenValid()
        {
            var id = Guid.NewGuid();
            var existing = new Person("Kristiyan", "Aleksandrov", "kris@gmail.com", "Dev") { Id = id };
            var req = new UpdatePersonRequest { FirstName = "Ivan", Position = "Lead Dev" };

            repoMock.Setup(r => r.GetAsync(id, ct)).ReturnsAsync(existing);
            repoMock.Setup(r => r.Update(existing));
            repoMock.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

            await peopleService.UpdateAsync(id, req, ct);

            Assert.Equal("Ivan", existing.FirstName);
            Assert.Equal("Lead Dev", existing.Position);
            repoMock.Verify(r => r.Update(existing), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFound_WhenPersonMissing()
        {
            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetAsync(id, ct)).ReturnsAsync((Person?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => peopleService.DeleteAsync(id, ct));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenPersonExists()
        {
            var id = Guid.NewGuid();
            var existing = new Person("Kristiyan", "Aleksandrov", "kris@gmail.com", "Dev") { Id = id };

            repoMock.Setup(r => r.GetAsync(id, ct)).ReturnsAsync(existing);
            repoMock.Setup(r => r.Delete(existing));
            repoMock.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

            await peopleService.DeleteAsync(id, ct);

            repoMock.Verify(r => r.Delete(existing), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedResponses()
        {
            var data = new List<Person>
            {
                new Person("Kristiyan", "Aleksandrov", "kris@gmail.com", "Dev"),
                new Person("Ivan", "Ivanov", "ivan@gmail.com", "QA")
            };
            repoMock.Setup(r => r.GetAllAsync(ct)).ReturnsAsync(data);

            var result = (await peopleService.GetAllAsync(ct)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Email == "kris@gmail.com");
            Assert.Contains(result, p => p.Email == "ivan@gmail.com");
        }
    }
}
