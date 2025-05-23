namespace ServiceContracts.DTOs.Dashboard
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public string OrganizationName { get; set; }
        
    }
}
