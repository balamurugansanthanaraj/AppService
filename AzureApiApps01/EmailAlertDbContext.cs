using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AzureApiApps01
{
    public class EmailAlertDbContext : DbContext
    {
        public DbSet<EmailAlert> EmailAlerts { get; set; }


        public EmailAlertDbContext(DbContextOptions<EmailAlertDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailAlert>().ToTable("EmailAlerts");
        }
    }
}
