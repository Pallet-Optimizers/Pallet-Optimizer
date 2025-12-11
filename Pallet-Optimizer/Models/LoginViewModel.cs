using System.ComponentModel.DataAnnotations;

namespace Pallet_Optimizer.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Brugernavn er påkrævet")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password er påkrævet")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}