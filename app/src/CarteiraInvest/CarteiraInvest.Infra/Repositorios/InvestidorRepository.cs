using CarteiraInvest.Domain.Aggregates;
using CarteiraInvest.Domain.Interfaces;
using CarteiraInvest.Infra.DataBaseContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Repositorios;

public sealed class InvestidorRepository(DbContextApplication context) : IInvestidorRepository
{
    private readonly DbContextApplication _context = context;

    public async Task<Investidor> CriarInvestidorAsync(Investidor investidor)
    {
        await _context.Investidores.AddAsync(investidor);
        await _context.SaveChangesAsync();
        return investidor;
    }
}
