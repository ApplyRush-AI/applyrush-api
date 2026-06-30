using Domain.Entities.Jobs.UserHiddenJobs;

namespace Application.Features.JobOffers.Data;

public sealed record UserHiddenJobInsertData(int UserId, int JobId) : IUserHiddenJobInsertData;
