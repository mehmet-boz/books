
using System.ComponentModel.DataAnnotations;

namespace books.Models.AdminViewModels;

public class UserVM
{
    [Required(ErrorMessage = "Kullanıcı adı boş geçilemez!")]
    [MinLength(3, ErrorMessage = "Kullanıcı adı en az 3 karakter olmalıdır!")]
    public string? username { get; set; }

    [Required(ErrorMessage = "Şifre alanı boş geçilemez!")]
    public string? password { get; set; }
}