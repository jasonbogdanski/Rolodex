using FluentAssertions;
using Rolodex.Web.Models;
using Rolodex.Web.Pages.Employee;
using Index = Rolodex.Web.Pages.Employee.Index;

namespace Rolodex.IntegrationTests.Pages.Employee;

[Collection(nameof(TestingFixture))]
public class IndexTests
{
    private readonly TestingFixture _fixture;

    public IndexTests(TestingFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_list_employees()
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

        var employeeId1 = await _fixture.SendAsync(command);

        command = new CreateEdit.Command
        {
            FirstName = "John",
            LastName = "Miller",
            JobTitle = "QA",
            Email = "qa@test.com",
            CompanyBranch = branch
        };

        var employeeId2 = await _fixture.SendAsync(command);

        var query = new Index.Query();

        var result = await _fixture.SendAsync(query);

        result.Should().NotBeNull();
        result.Employees.Count.Should().BeGreaterOrEqualTo(2);
        result.Employees.Select(m => m.Id).Should().Contain(employeeId1);
        result.Employees.Select(m => m.Id).Should().Contain(employeeId2);
    }
}
