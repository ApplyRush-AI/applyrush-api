using Application.Features.Authentication.Data;
using Application.Features.Resumes.Commands;
using Application.Identity;
using AutoMapper;
using DTO.MessageBroker.Messages.Resumes;
using Infrastructure.Identity;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Resumes;

public sealed class ResumeUploadedMessageConsumer : IConsumer<ResumeUploadedMessage>
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly IIdentityContextAccessor _identityContextAccessor;

    public ResumeUploadedMessageConsumer(
        ISender mediator,
        IMapper mapper,
        IIdentityContextAccessor identityContextAccessor)
    {
        _mediator = mediator;
        _mapper = mapper;
        _identityContextAccessor = identityContextAccessor;
    }

    public async Task Consume(ConsumeContext<ResumeUploadedMessage> context)
    {
        // Set the identity context to the uploading user so audit fields are attributed correctly
        // when the command persists the parsed profile.
        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfoById(context.Message.UserId));

        var command = _mapper.Map<ResumeParseCommand>(context.Message);
        await _mediator.Send(command, context.CancellationToken);
    }
}
