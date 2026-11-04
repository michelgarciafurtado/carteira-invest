using CarteiraInvest.Domain.Aggregates;
using CarteiraInvest.Infra.Identidade;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvest.Infra.DataBaseContext;

public class DbContextApplication:IdentityDbContext<ApplicationUser>
{
    public DbContextApplication(DbContextOptions<DbContextApplication> options):base(options)
    { }

    public DbSet<Investidor> Investidores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Investidor>().ToTable("Investidores");

        modelBuilder.ApplyConfiguration(new Configuracoes.InvestidorConfiguracoes());
    }
}
