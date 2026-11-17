using CarteiraInvest.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Interfaces.Servicos;

public interface IServicoCotacao
{
    Task<CotacaoDto> ObterCotacaoAsync(string Symbol, CancellationToken cancellationToken = default);
    Task<SymbolDto> ObterSymbolAsync(string Symbol, CancellationToken cancellationToken = default);
}
