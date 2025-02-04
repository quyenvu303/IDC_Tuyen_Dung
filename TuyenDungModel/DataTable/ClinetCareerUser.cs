using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ClinetCareerUser
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string CongViecGanDay { get; set; }
        public int? MucQuanTam { get; set; }
        public string Exp { get; set; }
        public string CtyGanNhat { get; set; }
        public int? BangCap { get; set; }
        public int? CapBacMongMuon { get; set; }
        public int? NgoaiNgu { get; set; }
        public int? CapBacNgoaiNgu { get; set; }
        public string ViTriMongMuon { get; set; }
        public string MucLuonMongMuon { get; set; }
        public int? HinhThucLamViec { get; set; }
        public int? NganhNgheKhac { get; set; }
        public int? NoiLamMongMuon { get; set; }
        public int? DiaChiMongMuon { get; set; }
        public int? NoiLamKhac { get; set; }
        public int? DiaChiKhac { get; set; }
        public string MucTieuNgheNghiep { get; set; }
        public string NguoiThamChieu { get; set; }
        public string KinhNghiem { get; set; }
        public string KyNang { get; set; }
        public string DinhKem { get; set; }
        public int? Status { get; set; }
    }
}
