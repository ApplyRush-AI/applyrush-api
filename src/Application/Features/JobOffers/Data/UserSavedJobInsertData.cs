using Domain.Entities.Jobs.UserSavedJobs;

namespace Application.Features.JobOffers.Data;

internal sealed record UserSavedJobInsertData(int UserId, int JobId) : IUserSavedJobInsertData;
