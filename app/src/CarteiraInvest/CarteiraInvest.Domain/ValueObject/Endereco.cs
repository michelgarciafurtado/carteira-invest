using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.ValueObject;

public record Endereco(string Logradouro, string Numero, string Cidade, string Estado, string CEP, string? Complemento = null);
