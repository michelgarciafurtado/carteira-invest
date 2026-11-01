using CarteiraInvest.Application.Interfaces.Identidade;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public class ServicoIdentidade : IServicoIdentidade
{
    public Task<ResultadoCriacaoUsuario> CriarUsuarioAsync(string email, string senha)
    {
        throw new NotImplementedException();
    }
}
