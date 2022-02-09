using System.ComponentModel.DataAnnotations;

namespace asp_net_mvc_t03.DTO.Frontend.Forms;

public class LoginForm
{
    [Required(ErrorMessage = "Email not specified")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password not specified")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}