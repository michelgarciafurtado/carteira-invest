using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public class CriarInvestidorDto
{
    public string Nome { get; set; } 
    public string Sobrenome { get; set; }
    public EnderecoDto Endereco { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
}

public record EnderecoDto(string Logradouro, string Numero, string Cidade, string Estado, string CEP, string? complemento = null);
