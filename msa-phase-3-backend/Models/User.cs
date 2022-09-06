using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace msa_phase_3_backend.Models;

public class User
{
    public User()
    {
        // Creates list of Pokemon on construction
        this.Pokemon = new List<Pokemon>();
    }

    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    public ICollection<Pokemon>? Pokemon { get; set; }
}

