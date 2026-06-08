using Application.Features.Users.Commands;
using Domain.Events.Users;
using DTO.MessageBroker.Messages.Search;
using MediatR;

namespace Application.Features.Users.EventHandlers;

public sealed class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly ISender _mediatr;

    public UserDeletedEventHandler(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _mediatr.Send(new UserIndexCommand(notification.User.Id), cancellationToken);
    }
}
