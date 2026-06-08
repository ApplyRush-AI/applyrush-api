using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums;
using DTO.Profile.JobFunctions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobFunctions.Queries;

public sealed record JobFunctionListGetQuery : IQuery<IReadOnlyList<JobFunctionItemResponse>>;

public sealed class JobFunctionListGetQueryHandler : IQueryHandler<JobFunctionListGetQuery, IReadOnlyList<JobFunctionItemResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobFunctionListGetQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<JobFunctionItemResponse>> Handle(JobFunctionListGetQuery request, CancellationToken cancellationToken)
    {
        var jobFunctions = await _dbContext.JobFunction
            .AsNoTracking()
            .Include(jf => jf.Children
                .Where(c => c.Status == Status.Active)
                .OrderBy(c => c.Name))
                .ThenInclude(c => c.Children
                    .Where(g => g.Status == Status.Active)
                    .OrderBy(g => g.Name))
            .Where(jf => jf.ParentId == null && jf.Status == Status.Active)
            .OrderBy(jf => jf.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<JobFunctionItemResponse>>(jobFunctions);
    }
}
