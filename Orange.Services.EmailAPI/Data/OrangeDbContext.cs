using Microsoft.EntityFrameworkCore;
using Orange.Services.EmailAPI.Models.Entity;

namespace Orange.Services.EmailAPI.Data
{
    public class OrangeDbContext : DbContext
    {
        public OrangeDbContext(DbContextOptions<OrangeDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
