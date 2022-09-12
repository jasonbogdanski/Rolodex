using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Rolodex.IntegrationTests.Fakes;
using Rolodex.Messages;
using Rolodex.Web.Models;
using Rolodex.Web.Services.Employee;

namespace Rolodex.IntegrationTests.Services.Employee;

[Collection(nameof(TestingFixture))]
public class SendEmployeeVerificationEmailTests
{
    private readonly TestingFixture _fixture;

    public SendEmployeeVerificationEmailTests(TestingFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_send_verification_email()
    {
        var branch = new CompanyBranch
        {
            Name = "Test Branch",
            City = "Akron",
            State = "Ohio"
        };

        var employee = new Web.Models.Employee
        {
            FirstName = "George",
            LastName = "Smith",
            JobTitle = "Accountant",
            Email = "test@test.com",
            CompanyBranch = branch
        };

        await _fixture.InsertAsync(employee);

        await _fixture.ExecuteScopeAsync(async provider =>
        {
            var messageSessionFake = provider.GetService<IMessageSession>() as MessageSessionFake;
            var mediator = provider.GetService<IMediator>();

            messageSessionFake!.VerifySend = o =>
            {
                var message = o as SendEmail;
                message.Should().NotBeNull();
                message?.Subject.Should().Be("Verify your email");
                message?.ToEmailAddress.Should().Be(employee.Email);
                message?.Body.Should().Contain("Verify your email by following the link https://localhost:7150/VerifyEmail?emailCode=dddddddd-dddd-dddd-dddd-dddddddddddd");
            };

            await mediator?.Send(new SendEmployeeVerificationEmail.Command
            {
                Id = employee.Id
            })!;

            return Task.CompletedTask;
        });

    }
}