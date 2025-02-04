using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class Jobs
    {
        public int Id { get; set; }
        public string CodeTuyenDung { get; set; }
        public string JobNameVi { get; set; }
        public string JobNameEn { get; set; }
        public int? HinhThucLamViec { get; set; }
        public int? CapBac { get; set; }
        public int? BangCap { get; set; }
        public string KinhNghiem { get; set; }
        public string KinhNghiemToiThieu { get; set; }
        public string KinhNghiemToiDa { get; set; }
        public int? PhongBanDangKy { get; set; }
        public string MucLuong { get; set; }
        public string NghanhNghe { get; set; }
        public int? SoLuong { get; set; }
        public string LyDoTuyen { get; set; }
        public int? NoiLamViec { get; set; }
        public int? BaoMatNoiLamViec { get; set; }
        public string MoTaJobs { get; set; }
        public string YeuCauJobs { get; set; }
        public int? ThoiGianTuyen { get; set; }
        public DateTime? BatDauTuyen { get; set; }
        public DateTime? KetThucTuyen { get; set; }
        public string Note { get; set; }
        public string JobTag { get; set; }
        public string PhucLoi { get; set; }
        public string TruongBoPhanTuyenDung { get; set; }
        public int? PhongBanYeuCau { get; set; }
        public string TruongBoPhanYeuCau { get; set; }
        public string NhanVienTuyen { get; set; }
        public int? NhanVienThayThe { get; set; }
        public int? View { get; set; }
        public int? UngVien { get; set; }
        public int? MailReply { get; set; }
        public int? StatusApprove { get; set; }
        public int? Status { get; set; }
    }
}
