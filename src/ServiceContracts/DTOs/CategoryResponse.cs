

namespace ServiceContracts.DTOs;

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public Uri? CategoryImage { get; set; }
    public string? CategoryName { get; set; }
}
