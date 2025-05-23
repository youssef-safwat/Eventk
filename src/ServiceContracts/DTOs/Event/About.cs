namespace ServiceContracts.DTOs.Event
{
    public class About
    {
        public string Description { get; set; }
        public IEnumerable<AboutCategory> aboutCategories { get; set; }
    }
    public class AboutCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}