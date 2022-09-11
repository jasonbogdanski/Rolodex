using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rolodex.Infrastructure;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rolodex.DataStore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Rolodex.Pages.Employee
{
    public class Delete : PageModel
    {
        private readonly IMediator _mediator;

        public Delete(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command Data { get; set; } = null!;

        public async Task OnGetAsync(Query query) => Data = await _mediator.Send(query);

        public async Task<ActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return this.RedirectToPageJson(nameof(Index));
        }

        public record Query : IRequest<Command>
        {
            public int? Id { get; init; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public record Command : IRequest
        {
            public int? Id { get; init; }
            public string FirstName { get; init; } = null!;
            public string LastName { get; init; } = null!;
            public string JobTitle { get; init; } = null!;
            public string Email { get; init; } = null!;
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateProjection<Models.Employee, Command>();
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

            public Task<Command> Handle(Query message, CancellationToken token) => _db
                .Employees
                .Where(i => i.Id == message.Id)
                .ProjectTo<Command>(_configuration)
                .SingleOrDefaultAsync(token)!;
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly RolodexContext _db;

            public CommandHandler(RolodexContext db) => _db = db;

            public async Task<Unit> Handle(Command message, CancellationToken token)
            {
                var employee = await _db.Employees
                    .Where(i => i.Id == message.Id)
                    .SingleAsync(token);

                _db.Employees.Remove(employee);

                return default;
            }
        }
    }
}
