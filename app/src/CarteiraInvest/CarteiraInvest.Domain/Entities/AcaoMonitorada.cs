using CarteiraInvest.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Entities;

public class AcaoMonitorada:BaseEntity
{
    public string Symbol { get; private set; }
    public Acao? Acao { get; private set; }
    public Guid InvestidorId { get; private set; }
    public Investidor? Investidor { get; private set; }
    public decimal? PrecoAlvo { get; private set; }
    public bool EmitirAlerta { get; private set; }

    public AcaoMonitorada(string symbol, Guid investidorId, decimal? precoAlvo, bool emitirAlerta)
    {
        if(string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol da ação não pode ser nulo ou vazio.", nameof(symbol));
        }
        if (investidorId == Guid.Empty)
        {
            throw new ArgumentException("O ID do investidor não pode ser vazio.", nameof(investidorId));
        }
        
        Symbol = symbol.Trim().ToUpperInvariant();
        InvestidorId = investidorId;
        PrecoAlvo = precoAlvo;
        EmitirAlerta = emitirAlerta;
    }
}
