using CarteiraInvest.Infra.DataBaseContext;
using CarteiraInvest.Infra.Identidade;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' nao configurada");

//Registra os serviços de proteção dos dados dos tokens do Identity
builder.Services.AddDataProtection();

builder.Services.AddDbContext<DbContextApplication>(options =>
options.UseSqlite(connectionString));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<DbContextApplication>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CarteiraInvest API",
        Description = "API for managing investment portfolios.",
        Contact = new OpenApiContact
        {
            Name = "Michel Garcia",
            Email = "michelgarciafurtado@gmail.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

