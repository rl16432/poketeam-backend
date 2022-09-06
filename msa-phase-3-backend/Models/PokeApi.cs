using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace msa_phase_3_backend.Models
{
    public class PokeApi
    {
        [JsonPropertyName("id")]
        [Required]
        public int PokemonId { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string? Name { get; set; }
        [JsonPropertyName("stats")]
        [Required]
        public ICollection<StatsApi>? Stats { get; set; }
    }

    public class StatsApi
    {
        [JsonPropertyName("base_stat")]
        public int BaseStat { get; set; }
        [JsonPropertyName("effort")]
        public long Effort { get; set; }
        [JsonPropertyName("stat")]
        public StatApi? Stat { get; set; }
    }

    public class StatApi
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
