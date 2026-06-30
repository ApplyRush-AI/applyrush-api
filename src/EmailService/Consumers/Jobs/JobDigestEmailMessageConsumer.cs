using DTO.MessageBroker.Messages.Jobs;
using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers.Jobs;

public sealed class JobDigestEmailMessageConsumer : IConsumer<JobDigestEmailMessage>
{
    private readonly ILogger<JobDigestEmailMessageConsumer> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ITemplateProvider _templateProvider;

    public JobDigestEmailMessageConsumer(
        ILogger<JobDigestEmailMessageConsumer> logger,
        IEmailSender emailSender,
        ITemplateProvider templateProvider)
    {
        _logger = logger;
        _emailSender = emailSender;
        _templateProvider = templateProvider;
    }

    public async Task Consume(ConsumeContext<JobDigestEmailMessage> context)
    {
        var message = context.Message;
        if (message.Jobs.Count == 0)
            return;

        _logger.LogInformation("Sending job digest with {Count} jobs to {Email}", message.Jobs.Count, message.Email);

        var model = new Dictionary<string, object?>
        {
            ["firstName"] = message.FirstName,
            ["jobs"] = message.Jobs
        };

        var htmlTemplateContent = await _templateProvider.RenderAsync("Job", "JobDigest", model);

        await _emailSender.SendAsync(new MailMessageRequest(
            message.Email,
            BuildSubject(message.Jobs),
            htmlTemplateContent,
            message.FirstName));
    }

    private static string BuildSubject(IReadOnlyList<JobDigestItem> jobs)
    {
        var top = jobs[0];
        return $"Latest \"{top.Title}\" jobs you might like — {top.MatchScore}% match from ApplyRush";
    }
}
