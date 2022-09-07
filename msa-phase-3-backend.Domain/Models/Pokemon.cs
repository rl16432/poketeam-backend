using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace msa_phase_3_backend.Domain.Models
{
    public class Pokemon : BaseModel
    {
        [Key]
        [Required]
        public new int Id { get; set; }
        [Required]
        public int PokemonNo { get; set; }
        [Required]
        public string? Name { get; set; }

        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public string? Image { get; set; }
    }

    public class Stats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PokemonId { get; set; }
        public int BaseStat { get; set; }
        public long Effort { get; set; }
        public Stat? Stat { get; set; }
    }

    public class Stat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
    }
}
