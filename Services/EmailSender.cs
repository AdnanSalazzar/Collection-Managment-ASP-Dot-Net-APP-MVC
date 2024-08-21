using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CollectionManagement.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, we just return a completed task.
            // In a real-world scenario, you would implement email sending logic here.
            return Task.CompletedTask;
        }
    }
}
