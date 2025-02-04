using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class CandidateHistoryApplyJobs
    {
        public int Id { get; set; }
        public int? CandidateId { get; set; }
        public DateTime? NgayUngTuyen { get; set; }
        public string Nguon { get; set; }
        public string TenCongViec { get; set; }
        public int? StatusProcessing { get; set; }
        public int? Status { get; set; }
    }
}
