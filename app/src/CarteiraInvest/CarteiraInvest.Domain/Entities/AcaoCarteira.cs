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
    public Acao? Acao { get; private set; }
    public DateTimeOffset? DataUltimaCotacao { get; private set; }
    public Money? PrecoUltimaCotacao { get; private set; }
    public Guid InvestidorId { get; private set; }
    public Investidor? Investidor { get; private set; }
    public Guid CorretoraId { get; private set; }
    public Corretora? Corretora { get; private set; }
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

        Symbol = symbol.Trim().ToUpperInvariant();
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

    public void AdicionarTransacao(TipoOperacaoEnum tipoOperacao,
                DateTime dataOperacao,
                decimal precoUnitario,
                int quantidade)
    {
     AcaoTransacao transacao = new AcaoTransacao(
            Symbol,
            InvestidorId,
            tipoOperacao,
            dataOperacao,
            precoUnitario,
            quantidade);

        if(transacao.PrecoUnitario.Valor <= 0)
            throw new ArgumentException("Preço unitário da transação deve ser maior que zero.", nameof(precoUnitario));
        if(transacao.Quantidade <= 0)
            throw new ArgumentException("Quantidade da transação deve ser maior que zero.", nameof(quantidade));
        if(transacao.DataOperacao > DateTime.UtcNow)
            throw new ArgumentException("Data da transação não pode ser no futuro.", nameof(dataOperacao));

        _transacoes.Add(transacao);

        //catch esta vazio porque as validações estao no metodo CalcularPrecoMedio
        try
        {
            RecalcularPosicao();
            RecalcularValoresMercado();
        }
        catch
        {
            _transacoes.Remove(transacao);
            throw ;
        } 
    }

    public void RecalcularPosicao()
    {
        decimal custoTotal = 0m;
        int quantidadeTotal = 0;
        decimal precoMedioAtual = 0m;
        foreach (var transacao in _transacoes.OrderBy(t => t.DataOperacao))
        {
            if (transacao.TipoOperacao == TipoOperacaoEnum.Compra)
            {
                custoTotal += transacao.PrecoUnitario.Valor * transacao.Quantidade;
                quantidadeTotal += transacao.Quantidade;
            }
            else if (transacao.TipoOperacao == TipoOperacaoEnum.Venda)
            {
                if (quantidadeTotal <= 0)
                {
                    throw new InvalidOperationException("Não é possível vender ações, pois não há ações disponíveis na carteira.");
                }
                if (transacao.Quantidade > quantidadeTotal)
                {
                    throw new InvalidOperationException("Não é possível vender mais ações do que a quantidade disponível na carteira.");
                }
                precoMedioAtual = custoTotal / quantidadeTotal;

                quantidadeTotal -= transacao.Quantidade;

                custoTotal = quantidadeTotal * precoMedioAtual;
            }
        }
        Quantidade = quantidadeTotal;
        PrecoMedio = Money.Criar(quantidadeTotal > 0 ? custoTotal / quantidadeTotal : 0m);
        ValorInvestido = Money.Criar(custoTotal);
    }

    public void RecalcularValoresMercado()
    {
        ValorAtual = CalcularValorAtual();
        LucroPrejuizo = CalcularLucroPrejuizo();
        PercentualLucroPrejuizo = CalcularPercentualLucroPrejuizo();
    }

    //calculos que dependem do preço atual da ação

    //calcula o valor atual da ação na carteira, multiplicando a quantidade de ações pelo preço da última cotação
    private Money CalcularValorAtual()
    {
        var preco = PrecoUltimaCotacao?.Valor ?? 0m;
        return Money.Criar(Quantidade * preco);
    }

    private Money CalcularLucroPrejuizo()
    {
        if(ValorInvestido.Valor == 0 || ValorAtual.Valor == 0 || PrecoUltimaCotacao == null)
            return Money.Criar(0);

        return ValorAtual - ValorInvestido;
    }

    private decimal CalcularPercentualLucroPrejuizo()
    {
        if (ValorInvestido.Valor == 0)
            return 0;

        return (LucroPrejuizo.Valor / ValorInvestido.Valor) * 100;
    }
}
