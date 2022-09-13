using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace msa_phase_3_backend.Domain.Models
{
    public class PokeApi
    {
        [JsonPropertyName("id")]
        [Required]
        public int PokemonId { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("stats")]
        [Required]
        public ICollection<StatsApi> Stats { get; set; } = new List<StatsApi>();
    }

    public class StatsApi
    {
        [JsonPropertyName("base_stat")]
        [Required]
        public int BaseStat { get; set; }
        [JsonPropertyName("effort")]
        [Required]
        public long Effort { get; set; }
        [JsonPropertyName("stat")]
        [Required]
        public StatApi Stat { get; set; } = new StatApi();
    }

    public class StatApi
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("url")]
        [Required]
        public string Url { get; set; } = string.Empty;
    }
}
