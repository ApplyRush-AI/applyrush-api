using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Features.Authentication.Data;
using Application.Identity;
using Domain.Interfaces;
using DTO.MessageBroker.Messages.Users;
using Infrastructure.Identity;
using MassTransit;

namespace Worker.Consumers.Users;

public sealed class ApplicationUserCreatedMessageConsumer : IConsumer<ApplicationUserCreatedMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityContextAccessor _identityContextAccessor;
    private readonly IDateTime _dateTime;

    public ApplicationUserCreatedMessageConsumer(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IIdentityContextAccessor identityContextAccessor,
        IDateTime dateTime)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _identityContextAccessor = identityContextAccessor;
        _dateTime = dateTime;
    }

    public async Task Consume(ConsumeContext<ApplicationUserCreatedMessage> context)
    {
        var userId = context.Message.UserId;
        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfoById(userId));
        await UserFoundationDataHelper.CreateAndSaveAsync(userId, _dbContext, _unitOfWork, _dateTime, context.CancellationToken);
    }
}
