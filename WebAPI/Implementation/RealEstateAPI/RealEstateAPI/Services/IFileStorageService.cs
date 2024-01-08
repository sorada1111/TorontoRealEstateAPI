namespace RealEstateAPI.Services
{
    public interface IFileStorageService
    {
        Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> propertyImageUrls);
        Task<String> SaveSingleImageAsync(IFormFile propertyImageUrl);
        Task DeleteImageAsync(string imageUrl);


    }
}
