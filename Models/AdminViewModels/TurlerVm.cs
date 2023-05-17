using System.ComponentModel.DataAnnotations;

namespace books.Models.AdminViewModels;

public class TurlerVm
{
    public int Id { get; set; }
    [Required]
    public string? TurAdi { get; set; }

    [Required]
    public int Sira { get; set; }
}