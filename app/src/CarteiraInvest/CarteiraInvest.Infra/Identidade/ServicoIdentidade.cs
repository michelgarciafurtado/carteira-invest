using CarteiraInvest.Application.Interfaces.Identidade;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public class ServicoIdentidade : IServicoIdentidade
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ServicoIdentidade(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResultadoCriacaoUsuario> CriarUsuarioAsync(string email, string senha)
    {
        var usuario = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var resultado = await _userManager.CreateAsync(usuario, senha);

        if(!resultado.Succeeded)
        {
            return new ResultadoCriacaoUsuario
            {
                Sucesso = false,
                Erros = resultado.Errors.Select(erro => erro.Description).ToArray()
            };
        }

        return new ResultadoCriacaoUsuario
        {
            Sucesso = true,
            UsuarioId = usuario.Id
        };

    }
}
