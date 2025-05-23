using Entites;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTOs;
using ServiceContracts.ServicesContracts;

namespace Services;
public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    public CategoryService(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    public async Task<ServiceResponse<ICollection<CategoryResponse>>> GetCategories()
    {
        try
        {
            var allCategories = await _context.Categories
                .AsNoTracking()
                .Select(c => c.Adapt<CategoryResponse>())
                .ToListAsync();

            return new ServiceResponse<ICollection<CategoryResponse>>
            {
                Data = allCategories,
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<ICollection<CategoryResponse>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ServiceStatus.InternalServerError
            };
        }
    }
}
