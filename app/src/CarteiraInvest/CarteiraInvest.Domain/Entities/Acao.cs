using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Entities;

public class Acao
{
    public Acao(string symbol, string longName)
    {
        if(string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol da ação não pode ser nulo ou vazio.", nameof(symbol));
        }
        if(string.IsNullOrWhiteSpace(longName))
        {
            throw new ArgumentException("Nome longo da ação não pode ser nulo ou vazio.", nameof(longName));
        }
        Symbol = symbol.ToUpper().Trim();
        LongName = longName.Trim();
    }

    public string Symbol { get; private set; }
    public string LongName { get; private set; }
}
