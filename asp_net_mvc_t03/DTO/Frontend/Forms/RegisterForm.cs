using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Frontend.Forms
{
    public class RegisterForm
    {
        [Required(ErrorMessage = "Email not specified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password not specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password entered incorrectly")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Name not specified")]
        public string Name { get; set; }
    }
}