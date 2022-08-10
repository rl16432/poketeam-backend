using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using msa_phase_2_backend.Models;
using msa_phase_2_backend.Models.DTO;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace msa_phase_2_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly UserContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(UserContext context, ILogger<UserController> logger, IHttpClientFactory clientFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (clientFactory is null)
        {
            throw new ArgumentNullException(nameof(clientFactory));
        }
        _client = clientFactory.CreateClient("pokeapi");
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>A response with the list of users</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users.Include("Pokemon").ToListAsync();
        return users;
    }

    /// <summary>
    /// Gets a single user
    /// </summary>
    /// <param name="userId">The userId of the user to return</param>
    /// <returns>The user with that userId, or a 404 response if the user does not exist</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetUser(int userId)
    {
        var user = await _context.Users.Include("Pokemon").FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="userCreateDto">The name of the user to create</param>
    /// <returns>A success code if the user has been created</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<User>> PostUser(UserCreateDTO userCreateDto)
    {
        var user = new User { UserName = userCreateDto.UserName, Pokemon = new List<Pokemon>() };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { userId = user.UserId, userName = user.UserName }, user);
    }

    /// <summary>
    /// Deletes a user and all of it's Pokemon if it exists
    /// </summary>
    /// <param name="userId">The userId of the user to delete</param>
    /// <returns>A 200 OK response</returns>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _context.Users.Include("Pokemon").FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null)
        {
            return Ok();
        }
        // Remove all Pokemon attributed to user
        foreach (Pokemon p in user.Pokemon!)
        {
            _context.Pokemon.Remove(p);
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

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
        var user = await _context.Users.Include("Pokemon").FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound("User does not exist");
        }

        var res = await _client.GetAsync($"/api/v2/pokemon/{pokemon.ToLower()}");

        if (!res.IsSuccessStatusCode)
        {
            return NotFound("No Pokemon found");
        }

        var content = await res.Content.ReadAsStringAsync();

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
            Name = jsonContent!.Name,
            Attack = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("attack"))!.BaseStat,
            Defense = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("defense"))!.BaseStat,
            Hp = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("hp"))!.BaseStat,
            SpecialAttack = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("special-attack"))!.BaseStat,
            SpecialDefense = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("special-defense"))!.BaseStat,
            Speed = jsonContent!.Stats!.FirstOrDefault(s => s!.Stat!.Name!.Equals("speed"))!.BaseStat
        };

        // Check if Pokemon already added to user
        if (user.Pokemon.Any(p => p.PokemonNo == newPokemon.PokemonNo))
        { 
            return BadRequest("Pokemon has already been added");
        }
        _context.Pokemon.Add(newPokemon);

        // Check if Pokemon already added in database
        //if (_context.Pokemon.Any(p => p.PokemonId == newPokemon.PokemonId) == false)
        //{
        //    _context.Pokemon.Add(newPokemon);
        //}

        //user.Pokemon.Add(new Pokemon
        //{
        //    PokemonId = jsonContent!.PokemonId,
        //    Name = jsonContent!.Name,
        //    Stats = jsonContent!.Stats!.Select(s => new Stats
        //    {
        //        BaseStat = s.BaseStat,
        //        Effort = s.Effort,
        //        Stat = new Stat
        //        {
        //            Name = s.Stat!.Name,
        //            Url = s.Stat!.Url
        //        }
        //    })
        //});

        user.Pokemon.Add(newPokemon);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a Pokemon from a user's favorites
    /// </summary>
    /// <param name="userId">The userId of which to delete the Pokemon from</param>
    /// <param name="pokemon">The name of the Pokemon to delete</param>
    /// <returns>A 200 OK Response</returns>
    [HttpDelete("{userId}/Pokemon")]
    public async Task<IActionResult> DeletePokemonFromUser(int userId, [Required] string pokemon)
    {
        var user = await _context.Users.Include("Pokemon").FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound("User does not exist");
        }

        // Remove Pokemon
        if (user.Pokemon!.Any(p => p.Name!.Equals(pokemon.ToLower())))
        {
            user.Pokemon!.Remove(user.Pokemon.First(p => p.Name!.Equals(pokemon.ToLower())));
            await _context.SaveChangesAsync();
        }
        return Ok();
    }
}