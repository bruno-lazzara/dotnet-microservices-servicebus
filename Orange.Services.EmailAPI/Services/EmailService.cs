using Microsoft.EntityFrameworkCore;
using Orange.Models.DTO;
using Orange.Services.EmailAPI.Data;
using Orange.Services.EmailAPI.Models.Entity;
using System.Text;

namespace Orange.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<OrangeDbContext> _dbOptions;

        public EmailService(DbContextOptions<OrangeDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            StringBuilder message = new();

            message.AppendLine("<br />Cart E-mail Requested");
            message.AppendLine($"<br />Total {cartDTO.CartHeader.CartTotal}");
            message.Append("<br />");
            message.Append("<ul>");
            foreach (var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append($"{item.Product.Name} x {item.Count}");
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                //TODO - send the e-mail using a service like SendGrid

                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message,
                };

                await using var _context = new OrangeDbContext(_dbOptions);

                await _context.EmailLoggers.AddAsync(emailLog);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
