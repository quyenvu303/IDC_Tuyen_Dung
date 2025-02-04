using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class CandidateHistoryRecruitment
    {
        public int Id { get; set; }
        public int? CandidateId { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? JobId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? Status { get; set; }
    }
}
