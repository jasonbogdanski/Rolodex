using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rolodex.DataStore;

namespace Rolodex.Pages.Employee
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; } = null!;

        public Create(IMediator mediator) => _mediator = mediator;

        public async Task<ActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson("Index");
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.FirstName).NotNull().Length(1, 255);
                RuleFor(m => m.LastName).NotNull().Length(1, 255);
                RuleFor(m => m.JobTitle).NotNull().Length(1, 255);
                RuleFor(m => m.Email).NotNull().EmailAddress().Length(1, 255);
            }
        }

        public record Command : IRequest<int>
        {
            public string FirstName { get; init; } = null!;
            public string LastName { get; init; } = null!;
            public string JobTitle { get; init; } = null!;
            public string Email { get; init; } = null!;
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly RolodexContext _context;

            public CommandHandler(RolodexContext context) => _context = context;

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                var employee = new Models.Employee
                {
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    JobTitle = message.JobTitle,
                    Email = message.Email
                };

                await _context.Employees.AddAsync(employee, token);

                await _context.SaveChangesAsync(token);

                return employee.Id;
            }
        }
    }
}
