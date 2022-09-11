using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Rolodex.Models;
using Rolodex.Pages.Employee;

namespace Rolodex.IntegrationTests.Pages.Employee;

[Collection(nameof(TestingFixture))]
public class CreateEditTests
{
    private readonly TestingFixture _fixture;

    public CreateEditTests(TestingFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_employee()
    {
        var branch = new CompanyBranch
        {
            Name = "Test Branch",
            City = "Akron",
            State = "Ohio"
        };

        await _fixture.InsertAsync(branch);

        var command = new CreateEdit.Command
        {
            FirstName = "George",
            LastName = "Smith",
            JobTitle = "Accountant",
            Email = "test@test.com",
            CompanyBranch = branch
        };

        var id = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.Employees.Include(x => x.CompanyBranch).Where(i => i.Id == id).SingleOrDefaultAsync());

        created.Should().NotBeNull();
        if (created != null)
        {
            created.FirstName.Should().Be(command.FirstName);
            created.LastName.Should().Be(command.LastName);
            created.JobTitle.Should().Be(command.JobTitle);
            created.Email.Should().Be(command.Email);
            created.CompanyBranch.Id.Should().Be(branch.Id);
        }
    }

    [Fact]
    public async Task Should_edit_existing_employee()
    {
        var branch1 = new CompanyBranch
        {
            Name = "Test Branch",
            City = "Akron",
            State = "Ohio"
        };

        var branch2 = new CompanyBranch
        {
            Name = "Test Branch",
            City = "Akron",
            State = "Ohio"
        };

        await _fixture.InsertAsync(branch1, branch2);

        var createCommand = new CreateEdit.Command
        {
            FirstName = "George",
            LastName = "Smith",
            JobTitle = "Accountant",
            Email = "test@test.com",
            CompanyBranch = branch1
        };

        var id = await _fixture.SendAsync(createCommand);

        var editCommand = new CreateEdit.Command
        {
            Id = id,
            FirstName = "Dude",
            LastName = "Cool",
            JobTitle = "No Job",
            Email = "cool.dude@hey.com",
            CompanyBranch = branch2
        };

        id = await _fixture.SendAsync(editCommand);

        var edited = await _fixture.ExecuteDbContextAsync(db => db.Employees.Include(x => x.CompanyBranch).Where(i => i.Id == id).SingleOrDefaultAsync());

        edited.Should().NotBeNull();
        if (edited != null)
        {
            edited.FirstName.Should().Be(editCommand.FirstName);
            edited.LastName.Should().Be(editCommand.LastName);
            edited.JobTitle.Should().Be(editCommand.JobTitle);
            edited.Email.Should().Be(editCommand.Email);
            edited.CompanyBranch.Id.Should().Be(editCommand.CompanyBranch.Id);
        }
    }
}