using System.ComponentModel.DataAnnotations;

namespace poketeam_backend.Domain.Models.DTO
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
    }
}
