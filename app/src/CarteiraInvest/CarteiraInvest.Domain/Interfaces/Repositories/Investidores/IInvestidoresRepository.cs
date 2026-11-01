using CarteiraInvest.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Interfaces.Repositories.Investidores;

public interface IInvestidoresRepository
{
    Task<Investidor?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Investidor?> ObterPorIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Investidor>> ListarAsync(CancellationToken cancellationToken = default);

    Task AdicionarAsync(Investidor investidor,CancellationToken cancellationToken = default);

    void Atualizar(Investidor investidor);

    void Remover(Investidor investidor);
}
