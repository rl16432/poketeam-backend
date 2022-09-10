using Microsoft.AspNetCore.Mvc;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Domain.Models.DTO;
using msa_phase_3_backend.Repository.Data;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using msa_phase_3_backend.Services.CustomServices;
using FluentValidation;

namespace msa_phase_3_backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IValidator<User> _userValidator;
    private readonly PokemonServices _pokemonService;
    private readonly UserServices _userService;

    public UserController(ILogger<UserController> logger, IHttpClientFactory clientFactory, IConfiguration configuration,
        UserServices userService, PokemonServices pokemonService, IValidator<User> userValidator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (clientFactory is null)
        {
            throw new ArgumentNullException(nameof(clientFactory));
        }
        _client = clientFactory.CreateClient("pokeapi");
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
        _pokemonService = pokemonService ?? throw new ArgumentNullException(nameof(pokemonService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>A response with the list of users</returns>
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    /// <summary>
    /// Gets a single user
    /// </summary>
    /// <param name="userName">The user name of the user to return</param>
    /// <returns>The user with that user name, or a 404 response if the user does not exist</returns>
    [HttpGet("{userName}")]
    public ActionResult<User> GetUser(string userName)
    {
        var user = _userService.GetByUserName(userName);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="userCreateDto">The name of the user to create</param>
    /// <returns>A 201 success code if the user has been created, along with the new user</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<User>> PostUser(UserCreateDTO userCreateDto)
    {
        var userExists = _userService.GetByUserName(userCreateDto.UserName);
        if (userExists != null)
        {
            return Conflict("Username already exists");
        }
        var user = new User { UserName = userCreateDto.UserName, Pokemon = new List<Pokemon>() };

        // FluentValidation to check the user name is valid
        FluentValidation.Results.ValidationResult result = await _userValidator.ValidateAsync(user);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(err => err.ErrorMessage));
        }
        _userService.Insert(user);

        return CreatedAtAction(nameof(PostUser), new { userId = user.Id, userName = user.UserName }, user);
    }

    /// <summary>
    /// Deletes a user and all of its Pokemon if it exists
    /// </summary>
    /// <param name="userId">The userId of the user to delete</param>
    /// <returns>A 200 OK response</returns>
    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        _userService.DeleteById(userId);

        return Ok();
    }

    /// <summary>
    /// Adds a specified Pokemon to a user's favorites
    /// </summary>
    /// <param name="userId">The userId of which to add the Pokemon to</param>
    /// <param name="pokemon">The name of the Pokemon to add</param>
    /// <returns>A 204 No Content Response</returns>
    [HttpPut("{userId}/Pokemon")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<User>> AddPokemonToUser(int userId, [Required] string pokemon)
    {
        // Find user by ID
        var user = _userService.Get(userId);

        if (user == null)
        {
            return NotFound("User does not exist");
        }

        if (user.Pokemon.Count >= 6)
        {
            return BadRequest("User already has 6 Pokemon");
        }

        // Call on PokeApi
        var res = await _client.GetAsync($"/api/v2/pokemon/{pokemon.ToLower()}");

        if (!res.IsSuccessStatusCode)
        {
            return NotFound("No Pokemon found");
        }

        var content = await res.Content.ReadAsStringAsync();
        // Convert JSON response to PokeApi schema object
        var jsonContent = JsonSerializer.Deserialize<PokeApi>(content);

        // Initialise list if not existant in user
        if (user.Pokemon == null)
        {
            user.Pokemon = new List<Pokemon>();
        }

        // Create new Pokemon object from API call
        var newPokemon = new Pokemon
        {
            PokemonNo = jsonContent!.PokemonId,
            Name = Regex.Replace(jsonContent!.Name!, @"(^\w)|(\s\w)", m => m.Value.ToUpper()),
            Attack = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("attack"))!.BaseStat,
            Defense = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("defense"))!.BaseStat,
            Hp = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("hp"))!.BaseStat,
            SpecialAttack = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("special-attack"))!.BaseStat,
            SpecialDefense = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("special-defense"))!.BaseStat,
            Speed = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("speed"))!.BaseStat
        };

        newPokemon.Image = $"{_configuration["PokemonArtworkAddress"]}/{newPokemon.PokemonNo}.png";

        // Check if Pokemon already added to user
        if (user.Pokemon.Any(p => p.PokemonNo == newPokemon.PokemonNo))
        {
            return BadRequest("Pokemon has already been added");
        }

        // Add Pokemon to database first
        _pokemonService.Insert(newPokemon);

        // Link new Pokemon to user
        user.Pokemon.Add(newPokemon);

        _userService.Update(user);

        return NoContent();
    }

    /// <summary>
    /// Deletes a Pokemon from a user's favorites
    /// </summary>
    /// <param name="userId">The userId of which to delete the Pokemon from</param>
    /// <param name="pokemon">The name of the Pokemon to delete</param>
    /// <returns>A 200 OK Response</returns>
    [HttpDelete("{userId}/Pokemon")]
    public IActionResult DeletePokemonFromUser(int userId, [Required] string pokemon)
    {
        // Find user by ID
        var user = _userService.Get(userId);

        if (user == null)
        {
            return NotFound("User does not exist");
        }
        else if (user.Pokemon == null || user.Pokemon.Count == 0)
        {
            return Ok();
        }
        // Remove Pokemon
        else
        {
            var pokemonObj = user.Pokemon.FirstOrDefault(p => p.Name!.ToLower().Equals(pokemon.ToLower()));
            if (pokemonObj != null)
            {
                user.Pokemon!.Remove(pokemonObj);
                _pokemonService.Delete(pokemonObj);
                _userService.Update(user);
            }
        }
        return Ok();
    }
}