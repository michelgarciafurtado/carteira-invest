using CarteiraInvest.Application.Dto;
using CarteiraInvest.Application.Interfaces.Servicos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Integracoes.Brapi;

public class ServicoCotacao : IServicoCotacao
{
    private readonly BrapiClient Brapi;

    public ServicoCotacao(BrapiClient brapi)
    {
        this.Brapi = brapi;
    }

    public async Task<CotacaoDto> ObterCotacaoAsync(string Symbol, CancellationToken cancellationToken = default)
    {
        var cotacao = await Brapi.GetQuotesAsync(Symbol, cancellationToken);
        var result = cotacao.FirstOrDefault();
                if (result == null)
                    throw new InvalidOperationException("Cotação não encontrada");
                return new CotacaoDto
                (result.Symbol, result.Data.Price);
    }

    public async Task<SymbolDto> ObterSymbolAsync(string Symbol, CancellationToken cancellationToken = default)
    {
        var result = await Brapi.GetSymbolAsync(Symbol, cancellationToken);
        return new SymbolDto
        (result.Symbol, result.Price, result.LongName);
    }
}
