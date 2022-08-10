using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace msa_phase_2_backend.Models
{
    public class Pokemon
    {
        [Key]
        public int PokemonId { get; set; }
        [Required]
        public int PokemonNo { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
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
