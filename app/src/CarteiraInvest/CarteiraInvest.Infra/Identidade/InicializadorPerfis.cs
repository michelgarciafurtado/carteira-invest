using CarteiraInvest.Application.Seguranca;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public static class InicializadorPerfis
{
    public static async Task InicializarAsync(RoleManager<IdentityRole> roleManager)
    {
       string[] perfis = [PerfisUsuarios.Administrador, PerfisUsuarios.Suporte, PerfisUsuarios.Investidor];

        foreach (string perfil in perfis)
        {
            if (!await roleManager.RoleExistsAsync(perfil))
            {
               var resultado = await roleManager.CreateAsync(new IdentityRole(perfil));

                if (!resultado.Succeeded)
                {
                    var erros = string.Join("; ", resultado.Errors.Select(e => e.Description));

                    throw new Exception($"Falha ao criar o perfil '{perfil}': {erros}");
                }
            }
        }
    }
}
