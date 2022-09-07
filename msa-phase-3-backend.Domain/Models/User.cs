using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace msa_phase_3_backend.Domain.Models;

public class User : BaseModel
{
    public User()
    {
        // Creates list of Pokemon on construction
        Pokemon = new List<Pokemon>();
    }

    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    public ICollection<Pokemon>? Pokemon { get; set; }
}

