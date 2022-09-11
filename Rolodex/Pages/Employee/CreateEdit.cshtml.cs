using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rolodex.DataStore;
using Rolodex.Infrastructure;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Rolodex.Pages.Employee
{
    public class CreateEdit : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; } = null!;

        public CreateEdit(IMediator mediator) => _mediator = mediator;

        public async Task OnGetCreateAsync() => Data = await _mediator.Send(new Query());

        public async Task<ActionResult> OnPostCreateAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson("Index");
        }

        public async Task<ActionResult> OnPostEditAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson("Index");
        }

        public async Task OnGetEditAsync(Query query) => Data = await _mediator.Send(query);

        public record Query : IRequest<Command>
        {
            public int? Id { get; init; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.FirstName).NotNull().Length(1, 255);
                RuleFor(m => m.LastName).NotNull().Length(1, 255);
                RuleFor(m => m.JobTitle).NotNull().Length(1, 255);
                RuleFor(m => m.Email).NotNull().EmailAddress().Length(1, 255);
                RuleFor(m => m.CompanyBranch).NotNull();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateProjection<Models.Employee, Command>();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly RolodexContext _db;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(RolodexContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query message, CancellationToken token)
            {
                Command model;
                if (message.Id == null)
                {
                    model = new Command();
                }
                else
                {
                    model = await _db.Employees
                        .Where(i => i.Id == message.Id)
                        .ProjectTo<Command>(_configuration)
                        .SingleOrDefaultAsync(token) ?? throw new InvalidOperationException();
                }

                return model;
            }
        }

        public record Command : IRequest<int>
        {
            public int? Id { get; init; }
            public string FirstName { get; init; } = null!;
            public string LastName { get; init; } = null!;
            public string JobTitle { get; init; } = null!;
            public string Email { get; init; } = null!;
            public Models.CompanyBranch CompanyBranch { get; init; } = null!;
        }

        public class CommandHandler : IRequestHandler<Command, int>
        {
            private readonly RolodexContext _context;

            public CommandHandler(RolodexContext context) => _context = context;

            public async Task<int> Handle(Command message, CancellationToken token)
            {
                Models.Employee employee;

                if (message.Id == null)
                {
                    employee = new Models.Employee();
                    await _context.Employees.AddAsync(employee, token);
                }
                else
                {
                    employee = await _context.Employees
                        .Where(x => x.Id == message.Id)
                        .SingleAsync(cancellationToken: token);
                }

                employee.Handle(message);

                await _context.SaveChangesAsync(token);

                return employee.Id;
            }
        }
    }
}
