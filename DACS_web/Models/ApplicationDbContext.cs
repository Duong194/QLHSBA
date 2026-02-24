using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DACS_web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<HoSoBenhAn> HoSoBenhAns { get; set; }
        public DbSet<DotKham> DotKhams { get; set; }
        public DbSet<CuocHenKham> CuocHenKhams { get; set; }
        public DbSet<Thuoc> Thuocs { get; set; }
        public DbSet<DonThuoc> DonThuocs { get; set; }
        public DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }
        public DbSet<ChanDoan> ChanDoans { get; set; }
        public DbSet<KetQuaKhamMat> KetQuaKhamMats { get; set; }
        public DbSet<XetNghiemCLS> XetNghiemCLSs { get; set; }
        public DbSet<Khoa> Khoas { get; set; }
        public DbSet<NhanVienYTe> NhanVienYTes { get; set; }
        public DbSet<PhuongPhapDieuTri> PhuongPhapDieuTris { get; set; }
        public DbSet<DanhMucICD> DanhMucICDs { get; set; }
        public DbSet<HinhAnhYTe> HinhAnhYTes { get; set; }
        public DbSet<TuVan> TuVans { get; set; }
        public DbSet<BaiBao> BaiBaos { get; set; }
        public DbSet<CaTruc> CaTrucs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTietDonThuoc>()
                .HasKey(c => new { c.MaDon, c.MaThuoc });

            modelBuilder.Entity<DotKham>()
                .HasOne(dk => dk.CuocHenKham)
                .WithMany(ch => ch.DotKhams)
                .HasForeignKey(dk => dk.MaCuocHen)
                .IsRequired(false);

            modelBuilder.Entity<DotKham>()
                .HasOne(d => d.KhoaTiepNhan)
                .WithMany()
                .HasForeignKey(d => d.MaKhoaTiepNhan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DotKham>()
                .HasOne(d => d.KhoaRaVien)
                .WithMany()
                .HasForeignKey(d => d.MaKhoaRaVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NhanVienYTe>()
                .HasOne(nv => nv.Khoa)
                .WithMany(k => k.NhanVienYTes)
                .HasForeignKey(nv => nv.MaKhoa);

            modelBuilder.Entity<HoSoBenhAn>()
                .HasOne(h => h.BacSiLapBenhAn)
                .WithMany()
                .HasForeignKey(h => h.MaBacSiLapBenhAn)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoSoBenhAn>()
                .HasOne(h => h.BacSiDieuTri)
                .WithMany()
                .HasForeignKey(h => h.MaBacSiDieuTri)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoSoBenhAn>()
                .HasOne(h => h.DotKham)
                .WithMany(d => d.HoSoBenhAns)
                .HasForeignKey(h => h.MaDotKham)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<ChiTietDonThuoc>()
                .HasKey(ct => new { ct.MaDon, ct.MaThuoc });

            modelBuilder.Entity<CaTruc>()
                .HasOne(ct => ct.NhanVien)
                .WithMany(nv => nv.CaTrucs)
                .HasForeignKey(ct => ct.MaNhanVien)
                .OnDelete(DeleteBehavior.Cascade);

            // ✨ MỚI - Cấu hình CuocHenKham relationships
            modelBuilder.Entity<CuocHenKham>()
                .HasOne(ch => ch.CaTruc)
                .WithMany()
                .HasForeignKey(ch => ch.MaCaTruc)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<CuocHenKham>()
                .HasOne(ch => ch.BacSiDuKien)
                .WithMany()
                .HasForeignKey(ch => ch.MaBacSiDuKien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CuocHenKham>()
                .HasOne(ch => ch.BenhNhan)
                .WithMany(bn => bn.CuocHenKhams)
                .HasForeignKey(ch => ch.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}