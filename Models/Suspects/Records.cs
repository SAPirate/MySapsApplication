namespace MySapsApplication.Models.Suspects
{
    public class Records
    {
        public IndexModel IndexModels { get; set; }
        public ICollection<CrimeRecordModel> Crime { get; set; }
        public CrimeRecordModel CrimeRecordModel { get; set; }
    }
}
