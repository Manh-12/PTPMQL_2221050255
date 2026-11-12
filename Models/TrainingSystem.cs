using System.ComponentModel.DataAnnotations;
namespace PTPMQL_MVC.Models;
public class TrainingSystem
{
    [Key, StringLength(2)] public string SystemCode { get; set; } = "";
    [Required, StringLength(100)] public string SystemName { get; set; } = "";
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
