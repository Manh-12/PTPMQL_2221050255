
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTPMQL_MVC.Models
{
    public class Student
    {
        // ==========================================
        // 1. MÃ SINH VIÊN
        // ==========================================

        [Key]
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [Display(Name = "Mã sinh viên")]
        public string StudentId { get; set; } = "";


        // ==========================================
        // 2. THÔNG TIN CÁ NHÂN
        // ==========================================

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(120, ErrorMessage = "Họ tên tối đa 120 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = "";


        [StringLength(250, ErrorMessage = "Địa chỉ tối đa 250 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = "";


        // ==========================================
        // 3. THÔNG TIN TUYỂN SINH
        // ==========================================

        [Required(ErrorMessage = "Vui lòng nhập năm tuyển sinh")]
        [Range(2000, 2099, ErrorMessage = "Năm tuyển sinh phải từ 2000 đến 2099")]
        [Display(Name = "Năm tuyển sinh")]
        public int AdmissionYear { get; set; }


        // ==========================================
        // 4. KHÓA NGOẠI HỆ ĐÀO TẠO
        // ==========================================

        [Required(ErrorMessage = "Vui lòng chọn hệ đào tạo")]
        [StringLength(2)]
        [Display(Name = "Hệ đào tạo")]
        public string SystemCode { get; set; } = "21";

        public TrainingSystem? TrainingSystem { get; set; }


        // ==========================================
        // 5. KHÓA NGOẠI NGÀNH HỌC
        // ==========================================

        [Required(ErrorMessage = "Vui lòng chọn ngành học")]
        [StringLength(2)]
        [Display(Name = "Ngành học")]
        public string MajorCode { get; set; } = "05";

        public Major? Major { get; set; }
    }
}
