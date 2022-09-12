using System.Reflection;
using msa_phase_3_backend.Domain.Models;
using msa_phase_3_backend.Repository.Data;
using Microsoft.EntityFrameworkCore;
using msa_phase_3_backend.Services;
using msa_phase_3_backend.Services.CustomServices;
using msa_phase_3_backend.Repository.Repository;
using FluentValidation;
using msa_phase_3_backend.Repository.IRepository;
using msa_phase_3_backend.Services.ICustomServices;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//// Configure local SQL server database
//builder.Services.AddDbContext<ApplicationDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
//);

if (builder.Configuration.GetConnectionString("DefaultConnection") == null)
{
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseInMemoryDatabase("PokeTeam")
    );
}
else
{
    // Configure local/Azure SQL server database
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
}

// Add dependencies for repository and DB services
builder.Services.AddScoped<IUserRepository<Trainer>, TrainerRepository>();
builder.Services.AddScoped<IRepository<Pokemon>, PokemonRepository>();

builder.Services.AddScoped<IUserCustomService<Trainer>, TrainerServices>();
builder.Services.AddScoped<ICustomService<Pokemon>, PokemonServices>();

// Add Fluent Validator
builder.Services.AddScoped<IValidator<Trainer>, TrainerValidator>();

// Add CORS
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Add HTTP Client
builder.Services.AddHttpClient(builder.Configuration["PokeapiClientName"] ?? "pokeapi", configureClient: client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PokeapiAddress"] ?? "https://pokeapi.co/api/v2");
});

var app = builder.Build();

// Add initial data to databases
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
