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
using CarteiraInvest.Infra.Servicos.Email;
using CarteiraInvest.Application.Interfaces.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
#region Configura o banco de dados e o Identity
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
#endregion

#region Registra os repositorios e UseCases
builder.Services.AddScoped<IInvestidorRepository, InvestidorRepository>();
builder.Services.AddScoped<IInvestidorUseCase, CriarInvestidorUseCase>();
#endregion

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

#region Registra os serviços
builder.Services.AddScoped<IServicoIdentidade, ServicoIdentidade>();
builder.Services.AddScoped<IServicoEmail, ServicoEmailGmail>();
builder.Services.AddOptions<ConfiguracaoEmail>()
    .Bind(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IServicoToken, ServicoToken>();
#endregion


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

var secaoJwt = builder.Configuration.GetSection("Jwt");

var configuracaoJwt = secaoJwt.Get<ConfiguracaoJwt>() ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

if( string.IsNullOrWhiteSpace(configuracaoJwt.Issuer) ||
    string.IsNullOrWhiteSpace(configuracaoJwt.Audience) || 
    configuracaoJwt.ExpiracaoEmMinutos <= 0)
{
    throw new InvalidOperationException("Configuração JWT inválida não encontrada.");
}

byte[] chaveJwt;

try
{
    chaveJwt = Convert.FromBase64String(configuracaoJwt.Chave);
}catch (FormatException)
{
    throw new InvalidOperationException("A chave JWT:Chave deve conter uma chave Base64 válida.");
}

if(chaveJwt.Length < 32)
{
    throw new InvalidOperationException("A chave JWT:Chave deve conter pelo menos 256 bits (32 bytes).");
}

builder.Services.AddOptions<ConfiguracaoJwt>()
    .Bind(secaoJwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuracaoJwt.Issuer,

            ValidateAudience = true,
            ValidAudience = configuracaoJwt.Audience,

            ValidateLifetime = true,
            RequireExpirationTime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chaveJwt),

            // Deve corresponder ao algoritmo usado no ServicoToken.
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],

            NameClaimType = "sub",
            RoleClaimType = "role",

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe somente o token JWT, sem o prefixo Bearer."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

