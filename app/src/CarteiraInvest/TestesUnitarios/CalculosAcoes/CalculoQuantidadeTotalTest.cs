using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios.CalculosAcoes;

public class CalculoQuantidadeTotalTest
{
    private AcaoCarteira _carteira = null!;
    private DateTime _dataBase;

    [SetUp]
    public void Setup()
    {
        _dataBase = DateTime.UtcNow.AddDays(-10);

        _carteira = new AcaoCarteira(
            "PETR4",
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    private void AdicionarTransacao(
        TipoOperacaoEnum tipo,
        int quantidade,
        int dias,
        decimal preco = 100m)
    {
        _carteira.AdicionarTransacao(
            tipo,
            _dataBase.AddDays(dias),
            preco,
            quantidade);
    }

    [Test]
    public void SemTransacoes_QuantidadeDeveSerZero()
    {
        Assert.That(_carteira.Quantidade, Is.EqualTo(0));
    }

    [Test]
    public void Compras_DevemSomarQuantidades()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 15, 1);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 20, 2);

        Assert.That(_carteira.Quantidade, Is.EqualTo(45));
    }

    [Test]
    public void VendaParcial_DeveReduzirQuantidade()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 45, 0);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 30, 1);

        Assert.That(_carteira.Quantidade, Is.EqualTo(15));
    }

    [Test]
    public void VendaTotal_DeveZerarQuantidade()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 10, 1);

        Assert.That(_carteira.Quantidade, Is.EqualTo(0));
    }

    [Test]
    public void CompraAposVendaTotal_DeveAtualizarQuantidade()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 10, 1);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 7, 2);

        Assert.That(_carteira.Quantidade, Is.EqualTo(7));
    }

    [Test]
    public void VendaInseridaForaDeOrdem_DeveConsiderarDataDaOperacao()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 20, 2);

        // Inserida por último, mas realizada entre as compras.
        AdicionarTransacao(TipoOperacaoEnum.Venda, 5, 1);

        Assert.That(_carteira.Quantidade, Is.EqualTo(25));
    }

    [Test]
    public void VendaMaiorQueSaldo_DevePreservarQuantidadeETransacoes()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10, 0);

        Assert.Throws<InvalidOperationException>(() =>
            AdicionarTransacao(TipoOperacaoEnum.Venda, 11, 1));

        Assert.That(_carteira.Quantidade, Is.EqualTo(10));
        Assert.That(_carteira.Transacoes.Count, Is.EqualTo(1));
    }

    [Test]
    public void VendaSemSaldo_DeveLancarExcecao()
    {
        Assert.Throws<InvalidOperationException>(() =>
            AdicionarTransacao(TipoOperacaoEnum.Venda, 1, 0));

        Assert.That(_carteira.Quantidade, Is.EqualTo(0));
        Assert.That(_carteira.Transacoes, Is.Empty);
    }
}