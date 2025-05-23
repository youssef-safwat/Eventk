using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using ServiceContracts.Options;
using Microsoft.AspNetCore.Http;
using ServiceContracts.ServicesContracts;

namespace Services;
public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _configuration;
    public CloudinaryService(IOptions<CloudinaryOptions> configuration)
    {
        _configuration = configuration.Value;

        Account account = new Account(
            _configuration.CloudName,
            _configuration.ApiKey,
            _configuration.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }
    public async Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            throw new ArgumentException("Public ID cannot be empty", nameof(publicId));

        var deletionParams = new DeletionParams(publicId)
        {
            Invalidate = true,
        };

        return await _cloudinary.DestroyAsync(deletionParams);
    }
    public async Task<Uri> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.SecureUrl;
    }
}

