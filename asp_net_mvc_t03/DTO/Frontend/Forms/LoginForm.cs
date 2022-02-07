using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Frontend.Forms
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Email not specified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password not specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}