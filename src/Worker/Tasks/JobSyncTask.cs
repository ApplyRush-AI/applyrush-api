using Application.Features.JobOffers.Commands;
using Infrastructure.Services.JobSync;
using Infrastructure.TaskScheduler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Worker.Tasks;

public sealed class JobSyncTask : ScheduledTaskBase
{
    protected override string Schedule => "0 0 */3 * *"; // every 3 days — conserves the 200 req/month free tier quota
    protected override string Name => "JobSync";

    public JobSyncTask(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    protected override async Task Run(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<JSearchOptions>>().Value;
        var mediator = serviceProvider.GetRequiredService<ISender>();

        await mediator.Send(new JobSyncCommand(options.PagesPerQuery));
    }
}
