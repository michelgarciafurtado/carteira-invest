using CarteiraInvest.Application.Dto;
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
    private readonly IServicoToken _servicoToken;
    private readonly IServicoEmail _servicoEmail;

    public ServicoIdentidade(UserManager<ApplicationUser> userManager, IServicoToken servicoToken, IServicoEmail servicoEmail)
    {
        _userManager = userManager;
        _servicoToken = servicoToken;
        _servicoEmail = servicoEmail;
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

    public async Task<bool> ConfirmarCadastroAsync(string usuarioId, string codigo)
    {
        if(string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(usuarioId))
            throw new ArgumentException("O código de confirmação e o ID do usuário não podem ser nulos ou vazios.", nameof(codigo));

        var usuario = await _userManager.FindByIdAsync(usuarioId);

        if(usuario is null)
            throw new InvalidOperationException($"Usuário com ID '{usuarioId}' não encontrado.");

        if(await _userManager.IsEmailConfirmedAsync(usuario))
            throw new InvalidOperationException($"O e-mail do usuário com ID '{usuarioId}' já foi confirmado.");

        var resultado = await _userManager.ConfirmEmailAsync(usuario, codigo);

        return resultado.Succeeded;
    }

    public async Task<UsuarioLogadoDto> AutenticarUsuario(string email, string senha)
    {
        var usuario = await _userManager.FindByEmailAsync(email);

        if (usuario is null)
            throw new InvalidOperationException("Email ou senha inválidos.");

        var passwordValid = await _userManager.CheckPasswordAsync(usuario, senha);

        if (!passwordValid)
            throw new InvalidOperationException("Email ou senha inválidos.");

        var EmailConfirmado = await _userManager.IsEmailConfirmedAsync(usuario);

        if (!EmailConfirmado)
            throw new InvalidOperationException("Email não confirmado.");

        var perfis = await _userManager.GetRolesAsync(usuario);

        var token = await _servicoToken.GerarTokenAsync(usuario.Id, usuario.Email, perfis);

        return new UsuarioLogadoDto(usuario.Id, token);
    }
}
