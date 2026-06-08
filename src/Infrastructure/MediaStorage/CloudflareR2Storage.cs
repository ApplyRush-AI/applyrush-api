using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.MediaStorage;
using Application.Common.MediaStorage.Interfaces;
using Domain.Entities.Medias;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.MediaStorage;

public class CloudflareR2Storage : IMediaStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly IMediaStorageHelper _mediaStorageHelper;
    private readonly MediaCloudflareR2Config _config;

    public CloudflareR2Storage(
        IAmazonS3 s3Client,
        IMediaStorageHelper mediaStorageHelper,
        IOptions<MediaConfig> configOpt)
    {
        _s3Client = s3Client;
        _mediaStorageHelper = mediaStorageHelper;
        _config = configOpt.Value.CloudflareR2!;
    }

    public async Task<Stream> Download(MediaItemKey itemKey)
    {
        var response = await _s3Client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = _config.BucketName,
            Key = BuildKey(itemKey)
        });
        return response.ResponseStream;
    }

    public async Task<string> Upload(MediaItemKey itemKey, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return await Upload(itemKey, stream, file.Length);
    }

    public async Task<string> Upload(MediaItemKey itemKey, Stream file, long length)
    {
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _config.BucketName,
            Key = BuildKey(itemKey),
            InputStream = file,
            UseChunkEncoding = false
        });
        return BuildPublicUrl(itemKey);
    }

    public Task<MediaItemKey> ParseUrl(string url) => Task.FromResult(_mediaStorageHelper.ParseUrl(url));

    public async Task Delete(MediaItemKey itemKey)
    {
        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _config.BucketName,
            Key = BuildKey(itemKey)
        });
    }

    private static string BuildKey(MediaItemKey itemKey)
        => $"{(int)itemKey.EntityType}/{itemKey.EntityId}/{itemKey.FileName}";

    private string BuildPublicUrl(MediaItemKey itemKey)
        => $"{_config.PublicUrl.TrimEnd('/')}/{BuildKey(itemKey)}";
}
