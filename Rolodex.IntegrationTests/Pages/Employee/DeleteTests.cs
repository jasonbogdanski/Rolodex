using Rolodex.Pages.Employee;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Rolodex.IntegrationTests.Pages.Employee;

[Collection(nameof(TestingFixture))]
public class DeleteTests
{
    private readonly TestingFixture _fixture;

    public DeleteTests(TestingFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_delete_employee()
    {
        var command = new CreateEdit.Command
        {
            FirstName = "George",
            LastName = "Smith",
            JobTitle = "Accountant",
            Email = "test@test.com"
        };

        var id = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.Employees.Where(i => i.Id == id).SingleOrDefaultAsync());

        created.Should().NotBeNull();

        await _fixture.SendAsync(new Delete.Command
        {
            Id = id
        });

        var deleted = await _fixture.ExecuteDbContextAsync(db => db.Employees.Where(i => i.Id == id).SingleOrDefaultAsync());

        deleted.Should().BeNull();
    }
}