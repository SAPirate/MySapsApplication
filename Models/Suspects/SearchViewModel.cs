namespace MySapsApplication.Models.Suspects
{
    public class SearchViewModel
    {
            public string Empsearch { get; set; }
            public List<IndexModel> IndexResults { get; set; } = new List<IndexModel>();
            public List<CrimeRecordModel> CrimeResults { get; set; } = new List<CrimeRecordModel>();
        

    }

}
