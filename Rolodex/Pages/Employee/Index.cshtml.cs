using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rolodex.DataStore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Rolodex.Pages.Employee
{
    public class Index : PageModel
    {
        private readonly IMediator _mediator;
        public Index(IMediator mediator) => _mediator = mediator;
        public Result Data { get; private set; }

        public async Task OnGetAsync() => Data = await _mediator.Send(new Query());

        public record Query : IRequest<Result>
        {
        }
        public record Result
        {
            public List<Employee> Employees { get; init; }

            public record Employee
            {
                public int Id { get; init; }
                public string FirstName { get; init; }
                public string LastName { get; init; }
                public string JobTitle { get; init; }
                public string Email { get; init; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateProjection<Models.Employee, Result.Employee>();
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly RolodexContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(RolodexContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken token)
            {
                var employees = await _db.Employees
                    .OrderBy(d => d.Id)
                    .ProjectTo<Result.Employee>(_configuration)
                    .ToListAsync(cancellationToken: token);

                return new Result
                {
                    Employees = employees
                };
            }
        }
    }
}
