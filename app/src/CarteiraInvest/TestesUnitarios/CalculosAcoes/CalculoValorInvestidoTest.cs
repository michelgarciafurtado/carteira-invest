using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios.CalculosAcoes;

[TestFixture]
public class CalculoValorInvestidoTest
{
    private AcaoCarteira _carteira = null!;
    private Guid _investidorId;
    private readonly DateTime _dataBase = new(2026, 10, 1);

    [SetUp]
    public void Setup()
    {
        _investidorId = Guid.NewGuid();

        _carteira = new AcaoCarteira(
            "PETR4",
            _investidorId,
            Guid.NewGuid());
    }

    private AcaoTransacao CriarTransacao(
        TipoOperacaoEnum tipo,
        decimal preco,
        int quantidade,
        int dias)
    {
        return new AcaoTransacao(
            "PETR4",
            _investidorId,
            tipo,
            _dataBase.AddDays(dias),
            preco,
            quantidade);
    }

    [Test]
    public void SemTransacoes_ValorInvestidoDeveSerZero()
    {
        _carteira.RecalcularPosicao();

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(0m));
    }

    [Test]
    public void Compra_DeveMultiplicarPrecoPelaQuantidade()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1000m));
    }

    [Test]
    public void Compras_DevemSomarCustos()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 120m, 15, 1));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 80m, 20, 2));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(4400m));
    }

    [TestCase(50)]
    [TestCase(150)]
    public void VendaParcial_DeveReduzirCustoPeloPrecoMedio(
        int precoVenda)
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 120m, 10, 1));

        // Custo: 2.200; quantidade: 20; preço médio: 110.
        _carteira.AdicionarTransacao(
            CriarTransacao(
                TipoOperacaoEnum.Venda, precoVenda, 5, 2));

        // Restam 15 ações com custo médio de 110.
        // O preço de venda não altera esse custo.
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1650m));
    }

    [Test]
    public void VendaTotal_DeveZerarValorInvestido()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Venda, 150m, 10, 1));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(0m));
    }

    [Test]
    public void CompraAposVendaTotal_DeveConsiderarSomenteNovoCusto()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Venda, 150m, 10, 1));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 80m, 7, 2));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(560m));
    }

    [Test]
    public void VendaInseridaForaDeOrdem_DeveConsiderarDataDaOperacao()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 200m, 10, 2));

        // Cronologicamente, a venda acontece antes da segunda compra.
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Venda, 150m, 5, 1));

        // Primeira compra após venda: 5 × 100 = 500.
        // Segunda compra: 10 × 200 = 2.000.
        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(2500m));
    }

    [Test]
    public void VendaComPrecoMedioPeriodico_DevePreservarPrecisao()
    {
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 10m, 1, 0));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 15m, 2, 1));

        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Venda, 20m, 1, 2));

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
        _carteira.AdicionarTransacao(
            CriarTransacao(TipoOperacaoEnum.Compra, 100m, 10, 0));

        var vendaInvalida =
            CriarTransacao(TipoOperacaoEnum.Venda, 150m, 11, 1);

        Assert.Throws<InvalidOperationException>(
            () => _carteira.AdicionarTransacao(vendaInvalida));

        Assert.That(
            _carteira.ValorInvestido.Valor,
            Is.EqualTo(1000m));

        Assert.That(_carteira.Transacoes.Count, Is.EqualTo(1));
    }
}
