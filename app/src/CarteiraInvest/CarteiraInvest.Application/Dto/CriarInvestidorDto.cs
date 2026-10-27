using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public class CriarInvestidorDto
{
    public string IdentityUserId { get; set; }
    public string Nome { get; set; } 
    public string Sobrenome { get; set; }
    public Endereco Endereco { get; set; }
}

public record Endereco(string Logradouro, string Numero, string? complemento, string Cidade, string Estado, string CEP);
