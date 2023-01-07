using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Report
{
    public class SavedReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Report Type")]
        public int ReportTypeId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }

        [Display(Name = "From")]
        public DateTime FromDate { get; set; }

        [Display(Name = "To")]
        public DateTime ToDate { get; set; }

        [ScaffoldColumn(false)]
        public string GeneratedQuery { get; set; }

        [Display(Name = "Saved By")]
        public int EmployeeId { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ReportTypeId")]
        public virtual ReportType ReportType { get; set; }
    }
}
