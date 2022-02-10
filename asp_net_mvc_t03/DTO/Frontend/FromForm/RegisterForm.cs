using System.ComponentModel.DataAnnotations;

namespace asp_net_mvc_t03.DTO.Frontend.FromForm;

public class RegisterForm
{
    [Required(ErrorMessage = "Email not specified")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password not specified")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password entered incorrectly")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name not specified")]
    public string Name { get; set; } = string.Empty;
}