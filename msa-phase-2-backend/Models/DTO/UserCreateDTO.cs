using System.ComponentModel.DataAnnotations;

namespace msa_phase_2_backend.Models.DTO
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
    }
}
