using System.ComponentModel.DataAnnotations;

namespace MySapsApplication.Models.Suspects
{
    public class CompletedCase
    {

        [Key]
            public int CrimeRecordId { get; set; } // This references the original case by its Id
            public string SuspectNo { get; set; }
            public string Offence { get; set; }
            public int Sentence { get; set; }
            public string IssuedAt { get; set; }
            public string IssuedBy { get; set; }
            public DateTime IssueDate { get; set; }
            public string Status { get; set; }
      
        


    }
}
