using Microsoft.EntityFrameworkCore;
using PTPMQL_MVC.Models;
namespace PTPMQL_MVC.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<TrainingSystem> TrainingSystems => Set<TrainingSystem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Faculty>().HasData(
            new Faculty { FacultyId="CNTT", FacultyName="Khoa Công nghệ thông tin" },
            new Faculty { FacultyId="DC", FacultyName="Khoa Địa chất" },
            new Faculty { FacultyId="DK", FacultyName="Khoa Dầu khí" },
            new Faculty { FacultyId="CD", FacultyName="Khoa Cơ - Điện" },
            new Faculty { FacultyId="TDBD", FacultyName="Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
            new Faculty { FacultyId="MT", FacultyName="Khoa Môi trường" },
            new Faculty { FacultyId="KT", FacultyName="Khoa Kinh tế và Quản trị kinh doanh" },
            new Faculty { FacultyId="XD", FacultyName="Khoa Xây dựng" },
            new Faculty { FacultyId="KHC", FacultyName="Khoa Khoa học cơ bản" }
        );
        modelBuilder.Entity<TrainingSystem>().HasData(
            new TrainingSystem { SystemCode="21", SystemName="Đại học chính quy" },
            new TrainingSystem { SystemCode="22", SystemName="Đại học liên thông" },
            new TrainingSystem { SystemCode="23", SystemName="Vừa làm vừa học" }
        );
        modelBuilder.Entity<Major>().HasData(
            new Major { MajorCode="05", MajorName="Công nghệ thông tin", FacultyId="CNTT" },
            new Major { MajorCode="01", MajorName="Kỹ thuật địa chất", FacultyId="DC" },
            new Major { MajorCode="02", MajorName="Kỹ thuật dầu khí", FacultyId="DK" },
            new Major { MajorCode="03", MajorName="Kỹ thuật cơ điện", FacultyId="CD" },
            new Major { MajorCode="04", MajorName="Trắc địa - Bản đồ", FacultyId="TDBD" },
            new Major { MajorCode="06", MajorName="Kỹ thuật môi trường", FacultyId="MT" },
            new Major { MajorCode="07", MajorName="Quản trị kinh doanh", FacultyId="KT" },
            new Major { MajorCode="08", MajorName="Kỹ thuật xây dựng", FacultyId="XD" }
        );
        modelBuilder.Entity<Student>()
            .HasOne(s => s.TrainingSystem).WithMany(t => t.Students)
            .HasForeignKey(s => s.SystemCode).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Major).WithMany(m => m.Students)
            .HasForeignKey(s => s.MajorCode).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Major>()
            .HasOne(m => m.Faculty).WithMany(f => f.Majors)
            .HasForeignKey(m => m.FacultyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Student>().HasIndex(s => s.StudentId).IsUnique();
    }
}
