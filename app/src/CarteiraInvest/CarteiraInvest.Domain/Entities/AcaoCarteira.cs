using CarteiraInvest.Domain.Aggregates;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CarteiraInvest.Domain.Entities;

public class AcaoCarteira:BaseEntity
{
    public string Symbol { get; private set; }
    public Acao Acao { get; private set; }
    public DateTimeOffset? DataUltimaCotacao { get; private set; }
    public Money? PrecoUltimaCotacao { get; private set; }
    public Guid InvestidorId { get; private set; }
    public Investidor Investidor { get; private set; }
    public Guid CorretoraId { get; private set; }
    public Corretora Corretora { get; private set; }
    public Money PrecoMedio { get; private set; }
    public int Quantidade { get; private set; }
    public Money ValorInvestido { get; private set; }
    public Money ValorAtual { get; private set; }
    public Money LucroPrejuizo { get; private set; }
    public decimal PercentualLucroPrejuizo { get; private set; }
    private readonly List<AcaoTransacao> _transacoes = [];
    public IReadOnlyCollection<AcaoTransacao> Transacoes
        => _transacoes.AsReadOnly();

    public AcaoCarteira(string symbol, Guid investidorId, Guid corretoraId)
    {
        if(string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("O símbolo da ação não pode ser nulo ou vazio.", nameof(symbol));
        
        if (investidorId == Guid.Empty)
            throw new ArgumentException("Investidor inválido.", nameof(investidorId));

        if (corretoraId == Guid.Empty)
            throw new ArgumentException("Corretora inválida.", nameof(corretoraId));

        Symbol = symbol.Trim().ToUpper();
        InvestidorId = investidorId;
        CorretoraId = corretoraId;
    }
    public void AtualizarCotacao(DateTimeOffset dataCotacao, decimal precoUltimaCotacao)
    {
        var dataUtc = dataCotacao.ToUniversalTime();

        if (DataUltimaCotacao.HasValue &&
            dataUtc < DataUltimaCotacao.Value)
        {
            throw new InvalidOperationException(
                "A cotação recebida é anterior à última registrada.");
        }

        if (precoUltimaCotacao <= 0)
            throw new ArgumentException("Preço da última cotação deve ser maior que zero.", nameof(precoUltimaCotacao));

        DataUltimaCotacao = dataCotacao;
        PrecoUltimaCotacao = Money.Criar(precoUltimaCotacao);

        RecalcularValoresMercado();
    }

    public void AdicionarTransacao(AcaoTransacao transacao)
    {
        if (transacao == null)
            throw new ArgumentNullException(nameof(transacao), "A transação não pode ser nula.");

        _transacoes.Add(transacao);

        //catch esta vazio porque as validações estao no metodo CalcularPrecoMedio
        try
        {
            PrecoMedio = CalcularPrecoMedio();
        }catch
        {
            _transacoes.Remove(transacao);
            throw ;
        }
        
        //RecalcularPosicao();
        //RecalcularValoresMercado();
    }

    public void RecalcularPosicao()
    {
        PrecoMedio = CalcularPrecoMedio();
        //Quantidade = CalcularQuantidade();
        //ValorInvestido = CalcularValorInvestido();
    }

    public void RecalcularValoresMercado()
    {
        ValorAtual = CalcularValorAtual();
        LucroPrejuizo = CalcularLucroPrejuizo();
        PercentualLucroPrejuizo = CalcularPercentualLucroPrejuizo();
    }

    private Money CalcularPrecoMedio()
    {
        decimal custoTotal = 0;
        int quantidadeTotal = 0;
        foreach (var transacao in _transacoes.OrderBy(t => t.DataOperacao))
        {
            if (transacao.TipoOperacao == TipoOperacaoEnum.Compra)
            {
                custoTotal += transacao.PrecoUnitario.Valor * transacao.Quantidade;
                quantidadeTotal += transacao.Quantidade;
            }
            else if (transacao.TipoOperacao == TipoOperacaoEnum.Venda)
            {
                if(quantidadeTotal <= 0)
                {
                    throw new InvalidOperationException("Não é possível vender ações, pois não há ações disponíveis na carteira.");
                }
                if (transacao.Quantidade > quantidadeTotal)
                {
                    throw new InvalidOperationException("Não é possível vender mais ações do que a quantidade disponível na carteira.");
                }
                decimal precoMedioAtual = custoTotal / quantidadeTotal;

                quantidadeTotal -= transacao.Quantidade;

                if(quantidadeTotal <= 0)
                    return Money.Zero;

                custoTotal = quantidadeTotal * precoMedioAtual;
            }
        }
        return Money.Criar(quantidadeTotal > 0 ? custoTotal / quantidadeTotal : 0);
    }

    private int CalcularQuantidade()
    {
        throw new NotImplementedException();
    }

    private Money CalcularValorInvestido()
    {
        throw new NotImplementedException();
    }
    
    //calculos que dependem do preço atual da ação
    private Money CalcularValorAtual()
    {
        throw new NotImplementedException();
    }

    private Money CalcularLucroPrejuizo()
    {
        throw new NotImplementedException();
    }

    private decimal CalcularPercentualLucroPrejuizo()
    {
        throw new NotImplementedException();
    }
}
