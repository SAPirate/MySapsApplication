using Microsoft.EntityFrameworkCore;

namespace MySapsApplication.Models.Suspects
{
    public class SuspectsDbContext :DbContext
    {
        public DbSet<IndexModel> IndexModels { get; set; }
        public DbSet<OffenceModel> OffenceModels { get; set; }
        public DbSet<CrimeRecordModel> CrimeRecordModels { get; set; }
      


        public SuspectsDbContext(DbContextOptions<SuspectsDbContext> options): base(options)
        {

        }

        internal async Task<string?> SearchRecordsAsync(string empsearch)
        {
            throw new NotImplementedException();
        }
    }
}
