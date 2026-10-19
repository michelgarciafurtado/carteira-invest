using CarteiraInvest.Domain.Aggregates;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Entities;

public class AcaoTransacao: BaseEntity
{
    public string AcaoSymbol { get; private set; }
    public Acao? Acao { get; private set; }
    public Guid InvestidorId { get; private set; }
    public Investidor? Investidor { get; private set; }
    public TipoOperacaoEnum TipoOperacao { get; private set; }
    public DateTime DataOperacao { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }

    public AcaoTransacao (
                string acaoSymbol,
                Guid investidorId,
                TipoOperacaoEnum tipoOperacao,
                DateTime dataOperacao,
                decimal precoUnitario,
                int quantidade)
    {
        if(string.IsNullOrWhiteSpace(acaoSymbol))
            throw new ArgumentException("O símbolo da ação não pode ser nulo ou vazio.", nameof(acaoSymbol));
        if (investidorId == Guid.Empty)
            throw new ArgumentException("O ID do investidor não pode ser vazio.", nameof(investidorId));
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade da transação deve ser um valor positivo.", nameof(quantidade));
        if (precoUnitario <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        AcaoSymbol = acaoSymbol.Trim().ToUpper();
        InvestidorId = investidorId;
        TipoOperacao = tipoOperacao;
        DataOperacao = dataOperacao;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }
}
