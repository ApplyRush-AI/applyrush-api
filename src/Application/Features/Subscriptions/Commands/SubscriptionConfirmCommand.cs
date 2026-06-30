using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Interfaces;
using DTO.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Commands;

public sealed record SubscriptionConfirmCommand(string SessionId) : ICommand<SubscriptionResponse>;

public sealed class SubscriptionConfirmCommandHandler : ICommandHandler<SubscriptionConfirmCommand, SubscriptionResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubscriptionConfirmCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IStripeService stripeService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SubscriptionResponse> Handle(SubscriptionConfirmCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw NotFoundException.New<UserSubscription>();

        var checkoutSession = await _stripeService.GetCheckoutSessionAsync(command.SessionId, cancellationToken);

        // The session must belong to this user's Stripe customer — guard against confirming someone else's session.
        if (checkoutSession.StripeCustomerId != subscription.StripeCustomerId)
            throw new ForbiddenAccessException();

        subscription.Update(new SubscriptionUpdateData(checkoutSession));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubscriptionResponse>(subscription);
    }
}
