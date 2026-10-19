using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.ValueObject;

public record Endereco(string Logradouro, string Numero, string complemento, string Cidade, string Estado, string CEP);
