using Application.Common.MediaStorage.Interfaces;

namespace Application.Common.MediaStorage;

public class MediaCloudflareR2Config : IMediaConfigStorageProvider
{
    public string AccountId { get; set; } = null!;
    public string AccessKeyId { get; set; } = null!;
    public string SecretAccessKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string PublicUrl { get; set; } = null!;
    public MediaStorageProviderType StorageProviderType => MediaStorageProviderType.CloudflareR2;
}
