using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class CandidateScheduleInterview
    {
        public int Id { get; set; }
        public int? CandidateId { get; set; }
        public string TieuDe { get; set; }
        public int? VongPhongVan { get; set; }
        public DateTime? ThoiGianPhongVan { get; set; }
        public int? NguoiPhongVan { get; set; }
        public string GhiChu { get; set; }
        public int? Status { get; set; }
    }
}
