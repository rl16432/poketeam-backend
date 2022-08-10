using System.Reflection;
using msa_phase_2_backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

if (builder.Environment.IsDevelopment())
{
    // Use in memory database if in development
    builder.Services.AddDbContext<UserContext>(opt =>
        opt.UseInMemoryDatabase("Users")
    );
}
else
{
    // Configure local SQL server database
    builder.Services.AddDbContext<UserContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
}

// Add HTTP Client
builder.Services.AddHttpClient(builder.Configuration["PokeapiClientName"], configureClient: client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PokeapiAddress"]);
});

var app = builder.Build();

// Add initial data to database
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
