using CarteiraInvest.Application.Dto;
using CarteiraInvest.Application.Interfaces.Identidade;
using CarteiraInvest.Application.Interfaces.Interfaces;
using CarteiraInvest.Domain.Aggregates;
using CarteiraInvest.Domain.Interfaces;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarteiraInvest.Application.UseCases.InvestidorUseCase;

public class CriarInvestidorUseCase(IInvestidorRepository investidorRepository,
                                    IServicoIdentidade servicoIdentidade) : IInvestidorUseCase
{
    private readonly IInvestidorRepository _investidorRepository = investidorRepository;
    private readonly IServicoIdentidade _servicoIdentidade = servicoIdentidade;

    public async Task<InvestidorDto> CriarInvestidorAsync(CriarInvestidorDto criarInvestidorDto)
    {
        if(criarInvestidorDto == null)
        {
            throw new ArgumentNullException(nameof(criarInvestidorDto));
        };
        if(string.IsNullOrWhiteSpace(criarInvestidorDto.Email))
        {
            throw new ArgumentException("O e-mail é obrigatório.", nameof(criarInvestidorDto.Email));
        }
        if(string.IsNullOrWhiteSpace(criarInvestidorDto.Senha))
        {
            throw new ArgumentException("A senha é obrigatória.", nameof(criarInvestidorDto.Senha));
        }
        if(ValidarSenha(criarInvestidorDto.Senha) == false)
        {
            throw new ArgumentException("A senha não atende aos critérios de segurança.", nameof(criarInvestidorDto.Senha));
        }
        if(ValidarEmail(criarInvestidorDto.Email) == false)
        {
            throw new ArgumentException("O e-mail não é válido.", nameof(criarInvestidorDto.Email));
        }
        var usuarioCriado = await _servicoIdentidade.CriarUsuarioAsync(criarInvestidorDto.Email, criarInvestidorDto.Senha);
        if(!usuarioCriado.Sucesso)
        {
            throw new Exception($"Falha ao criar usuário: {string.Join(", ", usuarioCriado.Erros)}");
        }
        var endereco = new Endereco(criarInvestidorDto.Endereco.Logradouro,
                                        criarInvestidorDto.Endereco.Numero,
                                        criarInvestidorDto.Endereco.complemento ?? string.Empty,
                                        criarInvestidorDto.Endereco.Cidade,
                                        criarInvestidorDto.Endereco.Estado,
                                        criarInvestidorDto.Endereco.CEP);

        var investidor = new Investidor(usuarioCriado.UsuarioId, 
                                        criarInvestidorDto.Nome,
                                        criarInvestidorDto.Sobrenome,
                                        endereco);
        var investidorCriado = await _investidorRepository.CriarInvestidorAsync(investidor);

        return new InvestidorDto(investidorCriado.IdentityUserId, investidorCriado.IdentityUserId);
    }

    private bool ValidarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var validador = new EmailAddressAttribute();

        return validador.IsValid(email);
    }

    private bool ValidarSenha(string? senha)
    {
        if (string.IsNullOrEmpty(senha))
            return false;

        bool temOitoCaracteres = senha.Length >= 8;
        bool temLetraMaiuscula = senha.Any(char.IsUpper);
        bool temCaractereEspecial = senha.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

        return temOitoCaracteres
            && temLetraMaiuscula
            && temCaractereEspecial;
    }
}
