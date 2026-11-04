using CarteiraInvest.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Interfaces;

public interface IInvestidorRepository
{
    Task<Investidor> CriarInvestidorAsync(Investidor investidor);
}
