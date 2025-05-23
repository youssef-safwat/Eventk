using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
namespace ServiceContracts.ServicesContracts;

public interface ICloudinaryService
{
    Task<DeletionResult> DeleteImageAsync(string publicId);
    Task<Uri> UploadImageAsync(IFormFile file);
}
