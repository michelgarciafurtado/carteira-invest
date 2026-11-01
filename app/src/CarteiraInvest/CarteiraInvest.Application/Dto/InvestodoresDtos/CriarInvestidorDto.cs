using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto.InvestodoresDtos;

public sealed record CriarInvestidorDto
(
    string Email,
    string Senha,
    string Nome,
    string Sobrenome,
    EnderecoDto Endereco
);

public sealed record EnderecoDto
(
    string Logradouro,
    string Numero,
    string Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep
);
