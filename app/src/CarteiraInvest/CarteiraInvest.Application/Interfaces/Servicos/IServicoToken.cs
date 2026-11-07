using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Interfaces.Servicos;

public interface IServicoToken
{
    Task<string> GerarTokenAsync(string id, string? email, IList<string> perfis);
}
