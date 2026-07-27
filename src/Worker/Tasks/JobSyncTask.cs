using Application.Features.JobOffers.Commands;
using Infrastructure.Services.JobSync;
using Infrastructure.TaskScheduler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Worker.Tasks;

public sealed class JobSyncTask : ScheduledTaskBase
{
    // Twice daily (00:00 and 12:00 UTC). ~19 category queries per run, so ~40 requests/day plus startup
    // runs — well within the JSearch Pro plan (10,000/month).
    protected override string Schedule => "0 0,12 * * *";
    protected override string Name => "JobSync";

    // Also run once on every Worker startup/restart, then follow the schedule above.
    protected override bool RunOnStartup => true;

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
