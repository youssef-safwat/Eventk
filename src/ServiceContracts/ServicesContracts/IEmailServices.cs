namespace ServiceContracts.ServicesContracts
{
    public interface IEmailServices
    {
        public Task SendEmail(string to, string subject, string body);
    }
}