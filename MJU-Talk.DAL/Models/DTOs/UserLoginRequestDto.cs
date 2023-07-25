using System.ComponentModel.DataAnnotations;

namespace MJU_Talk.DAL.Models.DTOs;

public class UserLoginRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

}
