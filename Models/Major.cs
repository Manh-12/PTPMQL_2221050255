using System.ComponentModel.DataAnnotations;
namespace PTPMQL_MVC.Models;
public class Major
{
    [Key, StringLength(2)] public string MajorCode { get; set; } = "";
    [Required, StringLength(150)] public string MajorName { get; set; } = "";
    [Required] public string FacultyId { get; set; } = "";
    public Faculty? Faculty { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
