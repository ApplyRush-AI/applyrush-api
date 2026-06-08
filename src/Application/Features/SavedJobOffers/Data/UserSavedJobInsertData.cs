using Domain.Entities.Jobs.UserSavedJobs;

namespace Application.Features.SavedJobOffers.Data;

internal sealed record UserSavedJobInsertData(int UserId, int JobId) : IUserSavedJobInsertData;
