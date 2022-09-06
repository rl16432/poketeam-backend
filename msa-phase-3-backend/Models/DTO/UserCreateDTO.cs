using System.ComponentModel.DataAnnotations;

namespace msa_phase_3_backend.Models.DTO
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
    }
}
