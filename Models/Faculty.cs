using System.ComponentModel.DataAnnotations;
namespace PTPMQL_MVC.Models;
public class Faculty
{
    [Key, StringLength(10)] public string FacultyId { get; set; } = "";
    [Required, StringLength(150)] public string FacultyName { get; set; } = "";
    public ICollection<Major> Majors { get; set; } = new List<Major>();
}
