using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using FluentValidation;

namespace msa_phase_3_backend.Domain.Models;

public class Trainer : BaseModel
{
    public Trainer()
    {
        // Creates list of Pokemon on construction
        Pokemon = new List<Pokemon>();
    }

    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    [Required]
    public string UserName { get; set; } = string.Empty;
    
    public ICollection<Pokemon> Pokemon { get; set; }
}

public class TrainerValidator : AbstractValidator<Trainer>
{
    public TrainerValidator()
    {
        // Username between 5 and 20 characters
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required").Length(5, 20);
        RuleFor(x => x.Pokemon).NotNull();
        RuleFor(x => x.Pokemon).Must(x => x!.Count <= 6).WithMessage("Trainer already has 6 Pokemon");
    }
}

