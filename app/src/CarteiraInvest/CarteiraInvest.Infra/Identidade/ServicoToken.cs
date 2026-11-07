using CarteiraInvest.Application.Interfaces.Servicos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public sealed class ServicoToken : IServicoToken
{
    public Task<string> GerarTokenAsync(string id, string? email, IList<string> perfis)
    {
        throw new NotImplementedException();
    }
}
