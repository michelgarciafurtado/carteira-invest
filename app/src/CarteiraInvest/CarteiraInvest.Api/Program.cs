using CarteiraInvest.Application.Interfaces.Identidade;
using CarteiraInvest.Application.Interfaces.Interfaces;
using CarteiraInvest.Application.UseCases.InvestidorUseCase;
using CarteiraInvest.Domain.Interfaces;
using CarteiraInvest.Infra.DataBaseContext;
using CarteiraInvest.Infra.Identidade;
using CarteiraInvest.Infra.Repositorios;
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
        options.User.RequireUniqueEmail = true;//nao permite que dois usuarios tenham o mesmo email
    })
    .AddRoles<IdentityRole>()//Habilita os perfis de usuarios
    .AddEntityFrameworkStores<DbContextApplication>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IServicoIdentidade, ServicoIdentidade>();
builder.Services.AddScoped<IInvestidorRepository, InvestidorRepository>();
builder.Services.AddScoped<IInvestidorUseCase, CriarInvestidorUseCase>();

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

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await InicializadorPerfis.InicializarAsync(roleManager);
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
app.MapHealthChecks("/health");

app.Run();

