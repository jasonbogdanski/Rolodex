using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Rolodex.Pages.Employee;

namespace Rolodex.IntegrationTests.Pages.Employee;

[Collection(nameof(TestingFixture))]
public  class CreateTests
{
    private readonly TestingFixture _fixture;

    public CreateTests(TestingFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_employee()
    {
        var command = new Create.Command
        {
            FirstName = "George",
            LastName = "Smith",
            JobTitle = "Accountant",
            Email = "test@test.com"
        };
        var id = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.Employees.Where(i => i.Id == id).SingleOrDefaultAsync());

        created.Should().NotBeNull();
        if (created != null)
        {
            created.FirstName.Should().Be(command.FirstName);
            created.LastName.Should().Be(command.LastName);
            created.JobTitle.Should().Be(command.JobTitle);
            created.Email.Should().Be(command.Email);
        }
    }
}