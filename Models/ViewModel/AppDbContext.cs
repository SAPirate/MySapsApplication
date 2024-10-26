using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MySapsApplication.Models.ViewModel
{
    public class AppDbContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            _options = options;

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
