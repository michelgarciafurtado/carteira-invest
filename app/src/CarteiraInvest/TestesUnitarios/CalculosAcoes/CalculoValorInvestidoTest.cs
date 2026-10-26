using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios.CalculosAcoes;

[TestFixture]
public class CalculoValorInvestidoTest
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
        decimal preco,
        int quantidade,
        int dias)
    {
        _carteira.AdicionarTransacao(
            tipo,
            _dataBase.AddDays(dias),
            preco,
            quantidade);
    }

    [Test]
    public void SemTransacoes_ValorInvestidoDeveSerZero()
    {
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(0m));
    }

    [Test]
    public void Compra_DeveMultiplicarPrecoPelaQuantidade()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1000m));
    }

    [Test]
    public void Compras_DevemSomarCustos()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 120m, 15, 1);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 80m, 20, 2);

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(4400m));
    }

    [TestCase(50)]
    [TestCase(150)]
    public void VendaParcial_DeveReduzirCustoPeloPrecoMedio(
        int precoVenda)
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 120m, 10, 1);

        // Custo: 2.200; quantidade: 20; preço médio: 110.
        AdicionarTransacao(TipoOperacaoEnum.Venda, precoVenda, 5, 2);

        // Restam 15 ações a 110, independentemente do preço de venda.
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1650m));
    }

    [Test]
    public void VendaTotal_DeveZerarValorInvestido()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 150m, 10, 1);

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(0m));
    }

    [Test]
    public void CompraAposVendaTotal_DeveConsiderarSomenteNovoCusto()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 150m, 10, 1);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 80m, 7, 2);

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(560m));
    }

    [Test]
    public void VendaInseridaForaDeOrdem_DeveConsiderarDataDaOperacao()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 200m, 10, 2);

        // Inserida por último, mas realizada entre as compras.
        AdicionarTransacao(TipoOperacaoEnum.Venda, 150m, 5, 1);

        // Após a venda: 5 × 100 = 500.
        // Segunda compra: 10 × 200 = 2.000.
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(2500m));
    }

    [Test]
    public void VendaComPrecoMedioPeriodico_DevePreservarPrecisao()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 10m, 1, 0);
        AdicionarTransacao(TipoOperacaoEnum.Compra, 15m, 2, 1);
        AdicionarTransacao(TipoOperacaoEnum.Venda, 20m, 1, 2);

        // Custo inicial: 40; quantidade inicial: 3.
        // Após vender uma ação, restam 2/3 do custo.
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(26.6666666666667m)
                .Within(0.000000000001m));
    }

    [Test]
    public void VendaInvalida_DevePreservarValorInvestido()
    {
        AdicionarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0);

        Assert.Throws<InvalidOperationException>(() =>
            AdicionarTransacao(TipoOperacaoEnum.Venda, 150m, 11, 1));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1000m));

        Assert.That(_carteira.Transacoes.Count, Is.EqualTo(1));
    }
}