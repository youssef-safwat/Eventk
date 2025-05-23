using ServiceContracts.DTOs;
namespace ServiceContracts.ServicesContracts;

public interface ICategoryService
{
    public Task<ServiceResponse<ICollection<CategoryResponse>>> GetCategories();
}
