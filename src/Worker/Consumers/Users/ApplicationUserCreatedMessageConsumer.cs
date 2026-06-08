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

    public ApplicationUserCreatedMessageConsumer(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IIdentityContextAccessor identityContextAccessor)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _identityContextAccessor = identityContextAccessor;
    }

    public async Task Consume(ConsumeContext<ApplicationUserCreatedMessage> context)
    {
        var userId = context.Message.UserId;
        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfoById(userId));
        await UserFoundationDataHelper.CreateAndSaveAsync(userId, _dbContext, _unitOfWork, context.CancellationToken);
    }
}
