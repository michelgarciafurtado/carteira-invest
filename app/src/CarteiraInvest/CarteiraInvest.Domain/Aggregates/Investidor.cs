using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Aggregates;

public class Investidor:BaseEntity
{
    public string IdentityUserId { get; private set; }
    public string Nome { get; private set; }
    public string Sobrenome { get; private set; }
    private readonly List<AcaoCarteira> _carteira = [];
    public IReadOnlyCollection<AcaoCarteira> Carteira => _carteira.AsReadOnly();
    private readonly List<AcaoMonitorada> _acoesMonitoradas = [];
    public IReadOnlyCollection<AcaoMonitorada> AcoesMonitoradas => _acoesMonitoradas.AsReadOnly();  
    public Endereco Endereco { get; private set; }

    public Investidor(string identityUserId, string nome, string sobrenome, Endereco endereco)
    {
        if(string.IsNullOrWhiteSpace(identityUserId))
            throw new ArgumentException("O ID do usuário não pode ser nulo ou vazio.", nameof(identityUserId));
        if(string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome não pode ser nulo ou vazio.", nameof(nome));
        if(string.IsNullOrWhiteSpace(sobrenome))
            throw new ArgumentException("O sobrenome não pode ser nulo ou vazio.", nameof(sobrenome));
        if(endereco == null)
            throw new ArgumentNullException("O endereço não pode ser nulo.", nameof(endereco));

        IdentityUserId = identityUserId;
        Nome = nome.Trim();
        Sobrenome = sobrenome.Trim();
        Endereco = endereco;
    }
    

    public void AtualizarEndereco(Endereco endereco)
    {
        Endereco = endereco;
    }

    public void AtualizarNome(string nome, string sobrenome)
    {
        Nome = nome;
        Sobrenome = sobrenome;
    }

    public void MonitorarAcao(AcaoMonitorada acaoMonitorada)
    {
        if(acaoMonitorada.InvestidorId != this.Id)
            throw new InvalidOperationException("A ação monitorada não pertence a este investidor.");
      
        if (AcoesMonitoradas.Any(a => string.Equals(a.Symbol, acaoMonitorada.Symbol)))
            throw new InvalidOperationException($"A ação {acaoMonitorada.Symbol} já está sendo monitorada.");
      
        _acoesMonitoradas.Add(acaoMonitorada);
    }

    public void PararMonitoramentoAcao(string symbol)
    {
        var acaoMonitorada = _acoesMonitoradas.FirstOrDefault(a => string.Equals(a.Symbol, symbol));
        if (acaoMonitorada == null)
            throw new InvalidOperationException($"A ação {symbol} não está sendo monitorada.");

        _acoesMonitoradas.Remove(acaoMonitorada);
    }

    public void AdicionarAcaoCarteira(AcaoCarteira acaoCarteira)
    {
       
        if (acaoCarteira.InvestidorId != this.Id)
            throw new InvalidOperationException("A ação da carteira não pertence a este investidor.");

        if (Carteira.Any(a => string.Equals(a.Symbol, acaoCarteira.Symbol) && a.CorretoraId == acaoCarteira.CorretoraId))
            throw new InvalidOperationException($"A ação {acaoCarteira.Symbol} já está na carteira do investidor para a corretora especificada.");
       
        _carteira.Add(acaoCarteira);
    }

    public AcaoCarteira? ObterAcaoCarteira(string symbol, Guid corretoraId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        return _carteira.FirstOrDefault(a =>
            string.Equals(
                a.Symbol,
                symbol,
                StringComparison.OrdinalIgnoreCase)
            && a.CorretoraId == corretoraId);
    }
}
