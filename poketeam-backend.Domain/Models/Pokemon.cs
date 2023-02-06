using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace poketeam_backend.Domain.Models
{
    public class Pokemon : BaseModel
    {
        [Key]
        [Required]
        public new int Id { get; set; }
        [Required]
        [JsonPropertyName("pokemonNo")]
        public int PokemonNo { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("hp")]
        public int Hp { get; set; }
        [JsonPropertyName("attack")]
        public int Attack { get; set; }
        [JsonPropertyName("defense")]
        public int Defense { get; set; }
        [JsonPropertyName("specialAttack")]
        public int SpecialAttack { get; set; }
        [JsonPropertyName("specialDefense")]
        public int SpecialDefense { get; set; }
        [JsonPropertyName("speed")]
        public int Speed { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;
    }
}
