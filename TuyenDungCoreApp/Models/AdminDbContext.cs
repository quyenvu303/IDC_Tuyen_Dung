using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TuyenDungModel.Custom;
using TuyenDungModel.DataTable;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungCoreApp.Models
{
    public partial class AdminDbContext : DbContext
    {
        public AdminDbContext()
        {
        }

        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApproveChain> ApproveChain { get; set; }
        public virtual DbSet<ApproveChainUser> ApproveChainUser { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<CandidateAttachment> CandidateAttachment { get; set; }
        public virtual DbSet<CandidateFolder> CandidateFolder { get; set; }
        public virtual DbSet<CandidateHistoryApplyJobs> CandidateHistoryApplyJobs { get; set; }
        public virtual DbSet<CandidateHistoryRecruitment> CandidateHistoryRecruitment { get; set; }
        public virtual DbSet<CandidateJobPhanLoai> CandidateJobPhanLoai { get; set; }
        public virtual DbSet<CandidateLog> CandidateLog { get; set; }
        public virtual DbSet<CandidateLogMail> CandidateLogMail { get; set; }
        public virtual DbSet<CandidateScheduleInterview> CandidateScheduleInterview { get; set; }
        public virtual DbSet<ClinetCareerUser> ClinetCareerUser { get; set; }
        public virtual DbSet<ClinetCvUser> ClinetCvUser { get; set; }
        public virtual DbSet<ClinetJobsUserApply> ClinetJobsUserApply { get; set; }
        public virtual DbSet<ClinetUser> ClinetUser { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<JobsApprove> JobsApprove { get; set; }
        public virtual DbSet<JobsConfigPhanLai> JobsConfigPhanLai { get; set; }
        public virtual DbSet<JobsLog> JobsLog { get; set; }
        public virtual DbSet<JobsSuitability> JobsSuitability { get; set; }
        public virtual DbSet<ManagementBangCap> ManagementBangCap { get; set; }
        public virtual DbSet<ManagementCapBac> ManagementCapBac { get; set; }
        public virtual DbSet<ManagementDoPhuHop> ManagementDoPhuHop { get; set; }
        public virtual DbSet<ManagementEmail> ManagementEmail { get; set; }
        public virtual DbSet<ManagementFolder> ManagementFolder { get; set; }
        public virtual DbSet<ManagementLanguage> ManagementLanguage { get; set; }
        public virtual DbSet<ManagementLocationWork> ManagementLocationWork { get; set; }
        public virtual DbSet<ManagementMenu> ManagementMenu { get; set; }
        public virtual DbSet<ManagementNganhNghe> ManagementNganhNghe { get; set; }
        public virtual DbSet<ManagementPermission> ManagementPermission { get; set; }
        public virtual DbSet<ManagementDepartment> ManagementDepartment { get; set; }
        public virtual DbSet<ManagementJobTitle> ManagementJobTitle { get; set; }
        public virtual DbSet<ManagementSettingRecruitment> ManagementSettingRecruitment { get; set; }
        public virtual DbSet<ManagementSocial> ManagementSocial { get; set; }
        public virtual DbSet<ManagementUser> ManagementUser { get; set; }
        public virtual DbSet<ManagementWebsiteBlog> ManagementWebsiteBlog { get; set; }
        public virtual DbSet<ManagementWebsiteContent> ManagementWebsiteContent { get; set; }
        public virtual DbSet<ManagementWebsiteMenu> ManagementWebsiteMenu { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }

        //----------------------------*Entity Custom*-------------------------------------
        public virtual DbSet<UserNoResult> UserNoResult { get; set; }
        public virtual DbSet<EmployeesViewLogin> EmployeesViewLogin { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Management_user_Get_Permission> Management_user_Get_Permissions { get; set; }
        public DbSet<Management_user_GetDepartment> Management_user_GetDepartment { get; set; }
        public DbSet<ManagementMenu_LoadMenu> ManagementMenu_LoadMenu { get; set; }
        public DbSet<ManagementDepartment_LoadDepartment> ManagementDepartment_LoadDepartment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DINHQUYEN\\SQLEXPRESS;Database=TuyenDungIDC;User Id=sa;Password=1234@;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApproveChain>(entity =>
            {
                entity.ToTable("Approve_chain");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.DepartmentName)
                    .HasColumnName("department_name")
                    .HasMaxLength(500);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(200);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ApproveChainUser>(entity =>
            {
                entity.ToTable("Approve_chain_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.JobsApproveId).HasColumnName("Jobs_approve_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UesrId).HasColumnName("uesr_id");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasMaxLength(200);

                entity.Property(e => e.BangCap).HasColumnName("bang_cap");

                entity.Property(e => e.CapBacHienTai).HasColumnName("cap_bac_hien_tai");

                entity.Property(e => e.CapBacMongMuon)
                    .HasColumnName("cap_bac_mong_muon")
                    .HasMaxLength(200);

                entity.Property(e => e.Cccd).HasColumnName("CCCD");

                entity.Property(e => e.CongViecGanNhat)
                    .HasColumnName("cong_viec_gan_nhat")
                    .HasMaxLength(200);

                entity.Property(e => e.CtyGanNhat)
                    .HasColumnName("cty_gan_nhat")
                    .HasMaxLength(200);

                entity.Property(e => e.Cv)
                    .HasColumnName("CV")
                    .HasMaxLength(200);

                entity.Property(e => e.DanhGia).HasColumnName("danh_gia");

                entity.Property(e => e.DienThoai)
                    .HasColumnName("Dien_thoai")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Exp).HasColumnName("exp");

                entity.Property(e => e.GioiTinh)
                    .HasColumnName("Gioi_tinh")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.HocVanBangCap).HasColumnName("hoc_van_bang_cap");

                entity.Property(e => e.HonNhan).HasColumnName("Hon_nhan");

                entity.Property(e => e.Huyen).HasMaxLength(50);

                entity.Property(e => e.KinhNghiemLamViec).HasColumnName("kinh_nghiem_lam_viec");

                entity.Property(e => e.KyNangNoiBat).HasColumnName("ky_nang_noi_bat");

                entity.Property(e => e.LoaiHoSo).HasColumnName("loai_ho_so");

                entity.Property(e => e.MucDoQuanTam).HasColumnName("muc_do_quan_tam");

                entity.Property(e => e.MucLuongMongMuon)
                    .HasColumnName("muc_luong_mong_muon")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MucTieuNgheNghiep).HasColumnName("muc_tieu_nghe_nghiep");

                entity.Property(e => e.NganhNgheMongMuon)
                    .HasColumnName("nganh_nghe_mong_muon")
                    .HasMaxLength(200);

                entity.Property(e => e.NganhNgheMongMuonKhac)
                    .HasColumnName("nganh_nghe_mong_muon_khac")
                    .HasMaxLength(200);

                entity.Property(e => e.NgayImport)
                    .HasColumnName("ngay_import")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgaySinh)
                    .HasColumnName("Ngay_sinh")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NgoaiNgu)
                    .HasColumnName("ngoai_ngu")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiThamChieu).HasColumnName("nguoi_tham_chieu");

                entity.Property(e => e.NguonImport).HasColumnName("nguon_import");

                entity.Property(e => e.NoiLamMongMuon).HasColumnName("noi_lam_mong_muon");

                entity.Property(e => e.NoiLamMongMuonKhac).HasColumnName("noi_lam_mong_muon_khac");

                entity.Property(e => e.NoiOHienTai)
                    .HasColumnName("Noi_o_hien_tai")
                    .HasMaxLength(500);

                entity.Property(e => e.QuocGia)
                    .HasColumnName("Quoc_gia")
                    .HasMaxLength(50);

                entity.Property(e => e.RaTruong).HasColumnName("ra_truong");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StatusProgress)
                    .HasColumnName("status_progress")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ten).HasMaxLength(200);

                entity.Property(e => e.TenDem)
                    .HasColumnName("Ten_dem")
                    .HasMaxLength(200);

                entity.Property(e => e.ThuMuc).HasColumnName("thu_muc");

                entity.Property(e => e.ThuongTru)
                    .HasColumnName("Thuong_tru")
                    .HasMaxLength(500);

                entity.Property(e => e.Tinh).HasMaxLength(50);

                entity.Property(e => e.TrinhDo)
                    .HasColumnName("trinh_do")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UrlFacebook)
                    .HasColumnName("url_Facebook")
                    .HasMaxLength(200);

                entity.Property(e => e.UrlLinkedin)
                    .HasColumnName("url_Linkedin")
                    .HasMaxLength(200);

                entity.Property(e => e.UrlSkype)
                    .HasColumnName("url_Skype")
                    .HasMaxLength(200);

                entity.Property(e => e.UserImport).HasColumnName("user_import");

                entity.Property(e => e.ViTriMongMuon)
                    .HasColumnName("vi_tri_mong_muon")
                    .HasMaxLength(200);

                entity.Property(e => e.ViTriUngTuyen).HasColumnName("vi_tri_ung_tuyen");
            });

            modelBuilder.Entity<CandidateAttachment>(entity =>
            {
                entity.ToTable("Candidate_attachment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment).HasMaxLength(200);

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<CandidateFolder>(entity =>
            {
                entity.ToTable("Candidate_folder");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<CandidateHistoryApplyJobs>(entity =>
            {
                entity.ToTable("Candidate_History_ApplyJobs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.NgayUngTuyen)
                    .HasColumnName("Ngay_ung_tuyen")
                    .HasColumnType("date");

                entity.Property(e => e.Nguon).HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StatusProcessing)
                    .HasColumnName("status_processing")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TenCongViec)
                    .HasColumnName("Ten_cong_viec")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<CandidateHistoryRecruitment>(entity =>
            {
                entity.ToTable("Candidate_History_Recruitment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.DenNgay)
                    .HasColumnName("Den_ngay")
                    .HasColumnType("date");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.NgayTao)
                    .HasColumnName("Ngay_tao")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TuNgay)
                    .HasColumnName("Tu_ngay")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<CandidateJobPhanLoai>(entity =>
            {
                entity.ToTable("Candidate_Job_phan_loai");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(200);

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.JobsConfigPhanLaiId).HasColumnName("Jobs_config_phan_lai_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StatusApprove).HasColumnName("status_approve");

                entity.Property(e => e.Step).HasColumnName("step");

                entity.Property(e => e.StepCurrent).HasColumnName("step_current");

                entity.Property(e => e.StepName)
                    .HasColumnName("step_name")
                    .HasMaxLength(200);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CandidateLog>(entity =>
            {
                entity.ToTable("Candidate_Log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TimeLog)
                    .HasColumnName("Time_log")
                    .HasColumnType("datetime");

                entity.Property(e => e.TitleLog)
                    .HasColumnName("title_log")
                    .HasMaxLength(200);

                entity.Property(e => e.UserLog).HasColumnName("user_log");
            });

            modelBuilder.Entity<CandidateLogMail>(entity =>
            {
                entity.ToTable("Candidate_log_mail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TimeLog)
                    .HasColumnName("Time_log")
                    .HasColumnType("datetime");

                entity.Property(e => e.TitleLog)
                    .HasColumnName("title_log")
                    .HasMaxLength(200);

                entity.Property(e => e.UserLog).HasColumnName("user_log");
            });

            modelBuilder.Entity<CandidateScheduleInterview>(entity =>
            {
                entity.ToTable("Candidate_Schedule_Interview");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CandidateId).HasColumnName("Candidate_id");

                entity.Property(e => e.GhiChu)
                    .HasColumnName("ghi_chu")
                    .HasMaxLength(200);

                entity.Property(e => e.NguoiPhongVan).HasColumnName("nguoi_phong_van");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ThoiGianPhongVan)
                    .HasColumnName("Thoi_gian_phong_van")
                    .HasColumnType("datetime");

                entity.Property(e => e.TieuDe)
                    .HasColumnName("Tieu_de")
                    .HasMaxLength(200);

                entity.Property(e => e.VongPhongVan).HasColumnName("vong_phong_van");
            });

            modelBuilder.Entity<ClinetCareerUser>(entity =>
            {
                entity.ToTable("clinet_career_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BangCap).HasColumnName("bang_cap");

                entity.Property(e => e.CapBacMongMuon).HasColumnName("cap_bac_mong_muon");

                entity.Property(e => e.CapBacNgoaiNgu).HasColumnName("cap_bac_ngoai_ngu");

                entity.Property(e => e.CongViecGanDay)
                    .HasColumnName("cong_viec_gan_day")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CtyGanNhat)
                    .HasColumnName("cty_gan_nhat")
                    .HasMaxLength(200);

                entity.Property(e => e.DiaChiKhac).HasColumnName("dia_chi_khac");

                entity.Property(e => e.DiaChiMongMuon).HasColumnName("dia_chi_mong_muon");

                entity.Property(e => e.DinhKem)
                    .HasColumnName("dinh_kem")
                    .HasMaxLength(200);

                entity.Property(e => e.Exp)
                    .HasColumnName("exp")
                    .HasMaxLength(50);

                entity.Property(e => e.HinhThucLamViec).HasColumnName("hinh_thuc_lam_viec");

                entity.Property(e => e.KinhNghiem).HasColumnName("kinh_nghiem");

                entity.Property(e => e.KyNang).HasColumnName("ky_nang");

                entity.Property(e => e.MucLuonMongMuon)
                    .HasColumnName("muc_luon_mong_muon")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MucQuanTam).HasColumnName("muc_quan_tam");

                entity.Property(e => e.MucTieuNgheNghiep).HasColumnName("muc_tieu_nghe_nghiep");

                entity.Property(e => e.NganhNgheKhac).HasColumnName("nganh_nghe_khac");

                entity.Property(e => e.NgoaiNgu).HasColumnName("ngoai_ngu");

                entity.Property(e => e.NguoiThamChieu).HasColumnName("nguoi_tham_chieu");

                entity.Property(e => e.NoiLamKhac).HasColumnName("noi_lam_khac");

                entity.Property(e => e.NoiLamMongMuon).HasColumnName("noi_lam_mong_muon");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ViTriMongMuon)
                    .HasColumnName("vi_tri_mong_muon")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ClinetCvUser>(entity =>
            {
                entity.ToTable("clinet_cv_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FileCv)
                    .HasColumnName("file_cv")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<ClinetJobsUserApply>(entity =>
            {
                entity.ToTable("clinet_jobs_user_apply");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<ClinetUser>(entity =>
            {
                entity.ToTable("clinet_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasMaxLength(200);

                entity.Property(e => e.Cccd).HasColumnName("CCCD");

                entity.Property(e => e.DiaChiHienTai)
                    .HasColumnName("dia_chi_hien_tai")
                    .HasMaxLength(200);

                entity.Property(e => e.DiaChiThuongTru)
                    .HasColumnName("dia_chi_thuong_tru")
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.GioiTinh).HasColumnName("gioi_tinh");

                entity.Property(e => e.Ho)
                    .HasColumnName("ho")
                    .HasMaxLength(50);

                entity.Property(e => e.HonNhan).HasColumnName("hon_nhan");

                entity.Property(e => e.Huyen).HasColumnName("huyen");

                entity.Property(e => e.NgaySinh)
                    .HasColumnName("ngay_sinh")
                    .HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(200);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.QuocGia).HasColumnName("quoc_gia");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Ten)
                    .HasColumnName("ten")
                    .HasMaxLength(20);

                entity.Property(e => e.ThanhPho).HasColumnName("thanh_pho");

                entity.Property(e => e.UrlFacebook)
                    .HasColumnName("url_facebook")
                    .HasMaxLength(200);

                entity.Property(e => e.UrlLinkedin)
                    .HasColumnName("url_linkedin")
                    .HasMaxLength(200);

                entity.Property(e => e.UrlSkype)
                    .HasColumnName("url_skype")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BangCap).HasColumnName("bang_cap");

                entity.Property(e => e.BaoMatNoiLamViec).HasColumnName("bao_mat_noi_lam_viec");

                entity.Property(e => e.BatDauTuyen)
                    .HasColumnName("bat_dau_tuyen")
                    .HasColumnType("datetime");

                entity.Property(e => e.CapBac).HasColumnName("cap_bac");

                entity.Property(e => e.CodeTuyenDung)
                    .HasColumnName("code_tuyen_dung")
                    .HasMaxLength(20);

                entity.Property(e => e.HinhThucLamViec).HasColumnName("hinh_thuc_lam_viec");

                entity.Property(e => e.JobNameEn)
                    .HasColumnName("job_name_en")
                    .HasMaxLength(500);

                entity.Property(e => e.JobNameVi)
                    .HasColumnName("job_name_vi")
                    .HasMaxLength(500);

                entity.Property(e => e.JobTag).HasColumnName("job_tag");

                entity.Property(e => e.KetThucTuyen)
                    .HasColumnName("ket_thuc_tuyen")
                    .HasColumnType("datetime");

                entity.Property(e => e.KinhNghiem)
                    .HasColumnName("kinh_nghiem")
                    .HasMaxLength(200);

                entity.Property(e => e.KinhNghiemToiDa)
                    .HasColumnName("kinh_nghiem_toi_da")
                    .HasMaxLength(20);

                entity.Property(e => e.KinhNghiemToiThieu)
                    .HasColumnName("kinh_nghiem_toi_thieu")
                    .HasMaxLength(20);

                entity.Property(e => e.LyDoTuyen)
                    .HasColumnName("ly_do_tuyen")
                    .HasMaxLength(500);

                entity.Property(e => e.MailReply).HasColumnName("mail_reply");

                entity.Property(e => e.MoTaJobs).HasColumnName("mo_ta_jobs");

                entity.Property(e => e.MucLuong)
                    .HasColumnName("muc_luong")
                    .HasMaxLength(200);

                entity.Property(e => e.NghanhNghe)
                    .HasColumnName("nghanh_nghe")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NhanVienThayThe).HasColumnName("nhan_vien_thay_the");

                entity.Property(e => e.NhanVienTuyen)
                    .HasColumnName("nhan_vien_tuyen")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NoiLamViec).HasColumnName("noi_lam_viec");

                entity.Property(e => e.PhongBanDangKy).HasColumnName("Phong_ban_dang_ky");

                entity.Property(e => e.PhongBanYeuCau).HasColumnName("phong_ban_yeu_cau");

                entity.Property(e => e.PhucLoi)
                    .HasColumnName("phuc_loi")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SoLuong).HasColumnName("so_luong");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StatusApprove)
                    .HasColumnName("status_approve")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ThoiGianTuyen).HasColumnName("thoi_gian_tuyen");

                entity.Property(e => e.TruongBoPhanTuyenDung)
                    .HasColumnName("truong_bo_phan_tuyen_dung")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TruongBoPhanYeuCau)
                    .HasColumnName("truong_bo_phan_yeu_cau")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UngVien).HasColumnName("ung_vien");

                entity.Property(e => e.View).HasColumnName("view");

                entity.Property(e => e.YeuCauJobs).HasColumnName("yeu_cau_jobs");
            });

            modelBuilder.Entity<JobsApprove>(entity =>
            {
                entity.ToTable("Jobs_approve");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(500);

                entity.Property(e => e.DateCreate)
                    .HasColumnName("date_create")
                    .HasColumnType("datetime");

                entity.Property(e => e.JobsId).HasColumnName("jobs_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StepApprove).HasColumnName("step_approve");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<JobsConfigPhanLai>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Jobs_config_phan_lai");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Step).HasColumnName("step");

                entity.Property(e => e.StepName)
                    .HasColumnName("step_name")
                    .HasMaxLength(200);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobsLog>(entity =>
            {
                entity.ToTable("Jobs_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DatetimeLog)
                    .HasColumnName("datetime_log")
                    .HasColumnType("datetime");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<JobsSuitability>(entity =>
            {
                entity.ToTable("Jobs_suitability");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BatBuoc).HasColumnName("bat_buoc");

                entity.Property(e => e.DoPhuHopId).HasColumnName("Do_phu_hop_id");

                entity.Property(e => e.ItQuanTrong).HasColumnName("it_quan_trong");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.QuanTrong).HasColumnName("quan_trong");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementBangCap>(entity =>
            {
                entity.ToTable("Management_Bang_cap");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CertificateNameEn)
                    .HasColumnName("certificate_name_en")
                    .HasMaxLength(200);

                entity.Property(e => e.CertificateNameVi)
                    .HasColumnName("certificate_name_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementCapBac>(entity =>
            {
                entity.ToTable("Management_Cap_bac");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RankNameEn)
                    .HasColumnName("rank_name_en")
                    .HasMaxLength(200);

                entity.Property(e => e.RankNameVi)
                    .HasColumnName("rank_name_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementDoPhuHop>(entity =>
            {
                entity.ToTable("Management_Do_phu_hop");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ManagementEmail>(entity =>
            {
                entity.ToTable("Management_email");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<ManagementFolder>(entity =>
            {
                entity.ToTable("Management_folder");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FolderName)
                    .HasColumnName("folder_name")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserCreator).HasColumnName("user_creator");
            });

            modelBuilder.Entity<ManagementLanguage>(entity =>
            {
                entity.ToTable("Management_Language");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ManagementLocationWork>(entity =>
            {
                entity.ToTable("Management_Location_work");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DistrictCity)
                    .HasColumnName("District_city")
                    .HasMaxLength(200);

                entity.Property(e => e.LocationName)
                    .HasColumnName("Location_name")
                    .HasMaxLength(200);

                entity.Property(e => e.OfficeNameEn)
                    .HasColumnName("Office_name_en")
                    .HasMaxLength(200);

                entity.Property(e => e.OfficeNameVi)
                    .HasColumnName("Office_name_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementMenu>(entity =>
            {
                entity.ToTable("Management_Menu");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasMaxLength(200);

                entity.Property(e => e.Areas)
                    .HasColumnName("areas")
                    .HasMaxLength(200);

                entity.Property(e => e.Controller)
                    .HasColumnName("controller")
                    .HasMaxLength(200);

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasMaxLength(200);

                entity.Property(e => e.MenuName)
                    .HasColumnName("menu_name")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ManagementNganhNghe>(entity =>
            {
                entity.ToTable("Management_Nganh_nghe");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IndustryEn)
                    .HasColumnName("Industry_en")
                    .HasMaxLength(200);

                entity.Property(e => e.IndustryVi)
                    .HasColumnName("Industry_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementPermission>(entity =>
            {
                entity.ToTable("Management_permission");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FullControl).HasColumnName("full_control");

                entity.Property(e => e.Access)
                    .HasColumnName("access")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Delete).HasColumnName("delete");

                entity.Property(e => e.Edit).HasColumnName("edit");

                entity.Property(e => e.Insert).HasColumnName("insert");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.View).HasColumnName("view");
            });

            modelBuilder.Entity<ManagementDepartment>(entity =>
            {
                entity.ToTable("Management_Department");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentCode)
                    .HasColumnName("department_code")
                    .HasMaxLength(50);

                entity.Property(e => e.DepartmentName)
                    .HasColumnName("department_name")
                    .HasMaxLength(200);

                entity.Property(e => e.ParenId).HasColumnName("paren_id");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasMaxLength(200);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ManagementJobTitle>(entity =>
            {
                entity.ToTable("Management_JobTitle");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PositionName)
                    .HasColumnName("PositionName")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });


            modelBuilder.Entity<ManagementSettingRecruitment>(entity =>
            {
                entity.ToTable("Management_Setting_Recruitment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StepName).HasMaxLength(200);
            });

            modelBuilder.Entity<ManagementSocial>(entity =>
            {
                entity.ToTable("Management_social");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LinkSocial)
                    .HasColumnName("link_social")
                    .HasMaxLength(200);

                entity.Property(e => e.SocialName)
                    .HasColumnName("social_name")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementUser>(entity =>
            {
                entity.ToTable("Management_User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("createdBy")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.DepartmentName)
                    .HasColumnName("department_name")
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(200);

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.JobTitleId).HasColumnName("jobTitle_id");

                entity.Property(e => e.JobTitleName)
                    .HasColumnName("jobTitle_name")
                    .HasMaxLength(200);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(200);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(200);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(200);

                entity.Property(e => e.user_no)
                    .HasColumnName("user_no")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ManagementWebsiteBlog>(entity =>
            {
                entity.ToTable("Management_website_blog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContentEn)
                    .HasColumnName("content_en")
                    .HasMaxLength(200);

                entity.Property(e => e.ContentVi)
                    .HasColumnName("content_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TitleEn)
                    .HasColumnName("title_en")
                    .HasMaxLength(200);

                entity.Property(e => e.TitleVi)
                    .HasColumnName("title_vi")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ManagementWebsiteContent>(entity =>
            {
                entity.ToTable("Management_website_content");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContentEn)
                    .HasColumnName("content_en")
                    .HasMaxLength(200);

                entity.Property(e => e.ContentVi)
                    .HasColumnName("content_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ManagementWebsiteMenu>(entity =>
            {
                entity.ToTable("Management_website_menu");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Banner)
                    .HasColumnName("banner")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MenuNameEn)
                    .HasColumnName("menu_name_en")
                    .HasMaxLength(200);

                entity.Property(e => e.MenuNameVi)
                    .HasColumnName("menu_name_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Status).HasColumnName("status");
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("createdBy")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiryDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.RevokedAt).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(200);

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
