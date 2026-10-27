using CarteiraInvest.Infra.Identidade;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvest.Infra.DataBaseContext;

public class DbContextApplication:IdentityDbContext<ApplicationUser>
{
    public DbContextApplication(DbContextOptions<DbContextApplication> options):base(options)
    { }
}
