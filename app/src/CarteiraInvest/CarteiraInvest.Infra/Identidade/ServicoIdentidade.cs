using CarteiraInvest.Application.Interfaces.Identidade;
using CarteiraInvest.Application.Interfaces.Servicos;
using CarteiraInvest.Application.Seguranca;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public class ServicoIdentidade : IServicoIdentidade
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServicoEmail _servicoEmail;

    public ServicoIdentidade(UserManager<ApplicationUser> userManager, IServicoEmail servicoEmail)
    {
        _userManager = userManager;
        _servicoEmail = servicoEmail;
    }

    public Task<bool> ConfirmarCadastroAsync(string usuarioId, string codigo)
    {
        throw new NotImplementedException();
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

        var perfil = await _userManager.AddToRoleAsync(usuario, PerfisUsuarios.Investidor);

        if(!perfil.Succeeded)
        {
            return new ResultadoCriacaoUsuario
            {
                Sucesso = false,
                Erros = perfil.Errors.Select(erro => erro.Description).ToArray()
            };
        }

        return new ResultadoCriacaoUsuario
        {
            Sucesso = true,
            UsuarioId = usuario.Id
        };

    }

    public async Task EnviarConfirmacaoCadastroAsync(string usuarioId)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException($"Usuário com ID '{usuarioId}' não encontrado.");

        if(await _userManager.IsEmailConfirmedAsync(usuario))
            throw new InvalidOperationException($"O e-mail do usuário com ID '{usuarioId}' já foi confirmado.");

        if (string.IsNullOrWhiteSpace(usuario.Email))
            throw new InvalidOperationException($"O usuário com ID '{usuarioId}' não possui um e-mail válido.");

        var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);

        var mensagem = $"""
        Olá!

        Para confirmar seu cadastro no CarteiraInvest,
        copie o código abaixo e informe na aplicação:

        {codigo}

        O código é válido por 2 horas.

        Se você não solicitou este cadastro, ignore esta mensagem.
        """;

        await _servicoEmail.EnviarAsync(usuario.Email, "Confirmação de Cadastro - CarteiraInvest", mensagem, CancellationToken.None);
    }
}
