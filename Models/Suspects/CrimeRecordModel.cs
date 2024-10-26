using System.ComponentModel.DataAnnotations;
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySapsApplication.Models.Suspects
{
    public class CrimeRecordModel
    {
       
        public int Id { get; set; } 

        [Required(ErrorMessage = "Enter Suspect key Number.")]
        public string? SuspectNo { get; set; }

        [Required(ErrorMessage = "Criminal Offence is required.")]
        public string? Offence { get; set; }

        [Required(ErrorMessage = "Sentence years is required.")]
        public int? Sentence { get; set; }

        [Required(ErrorMessage = "IssuedAt location is required.")]
        public string? IssuedAt { get; set; }

        [Required(ErrorMessage = "IssuedBy police officer Name is required.")]
        public string? IssuedBy { get; set; }

        [Required(ErrorMessage = "Issued Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }
        public string? Status { get; set; }


    }
}
