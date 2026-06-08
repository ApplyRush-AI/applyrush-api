using Domain.Entities.Base;
using Domain.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.RefreshTokens
{
    public class RefreshToken : IEntity
    {
        [Key]
        public string Value { get; private set; } = null!;
        public int UserId { get; private set; }
        public DateTime ExpiryTime { get; private set; }
        public string? DeviceInfo { get; private set; }
        public string? IpAddress { get; private set; }
        public DateTime? LastUsedAt { get; private set; }

        public virtual ApplicationUser User { get; set; } = null!;

        private RefreshToken()
        {
            
        }

        private RefreshToken(
            string value,
            int userId,
            DateTime expiryTime,
            string? deviceInfo,
            string? ipAddress)
        {
            Value = value;
            UserId = userId;
            ExpiryTime = expiryTime;
            DeviceInfo = deviceInfo;
            IpAddress = ipAddress;
        }

        public static RefreshToken Create(
            string value,
            int userId,
            DateTime expiryTime,
            string? deviceInfo = null,
            string? ipAddress = null)
        {
            return new RefreshToken(value, userId, expiryTime, deviceInfo, ipAddress);
        }

        public void UpdateLastUsed(DateTime lastUsedAt)
        {
            LastUsedAt = lastUsedAt;
        }
    }
}

