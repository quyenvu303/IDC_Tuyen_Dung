using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ClinetUser
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string Password { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? GioiTinh { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? QuocGia { get; set; }
        public int? ThanhPho { get; set; }
        public int? Huyen { get; set; }
        public string DiaChiHienTai { get; set; }
        public string DiaChiThuongTru { get; set; }
        public int? Cccd { get; set; }
        public int? HonNhan { get; set; }
        public string UrlLinkedin { get; set; }
        public string UrlFacebook { get; set; }
        public string UrlSkype { get; set; }
        public int? Status { get; set; }
    }
}
