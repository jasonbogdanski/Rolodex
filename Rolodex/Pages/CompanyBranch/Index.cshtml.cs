using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rolodex.DataStore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Rolodex.Pages.CompanyBranch;

public class Index : PageModel
{
    private readonly IMediator _mediator;
    public Index(IMediator mediator) => _mediator = mediator;
    public Result Data { get; private set; } = null!;

    public async Task OnGetAsync() => Data = await _mediator.Send(new Query());

    public record Query : IRequest<Result>
    {
    }
    public record Result
    {
        public List<CompanyBranch> CompanyBranches { get; init; } = null!;

        public record CompanyBranch
        {
            public int Id { get; init; }
            public string Name { get; set; } = null!;
            public string City { get; set; } = null!;
            public string State { get; set; } = null!;
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateProjection<Models.CompanyBranch, Result.CompanyBranch>();
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
            var companyBranches = await _db.CompanyBranches
                .OrderBy(d => d.Id)
                .ProjectTo<Result.CompanyBranch>(_configuration)
                .ToListAsync(cancellationToken: token);

            return new Result
            {
                CompanyBranches = companyBranches
            };
        }
    }
}