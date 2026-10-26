using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios.CalculosAcoes;

public class CalculoPrecoMedioTest
{
    private AcaoCarteira petrobras = null!;
    private DateTime dataReferencia;

    [SetUp]
    public void Setup()
    {
        petrobras = new AcaoCarteira(
            "PETR4",
            Guid.NewGuid(),
            Guid.NewGuid());

        dataReferencia = DateTime.UtcNow.AddDays(-2);
    }

    [Test]
    public void CalcularPrecoMedio_AposComprasVendaENovaCompra_DeveRetornarValorEsperado()
    {
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-10),
            100m,
            10);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-5),
            120m,
            15);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-2),
            80m,
            20);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Venda,
            dataReferencia,
            115m,
            30);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(1),
            105m,
            20);

        Assert.That(
            Math.Round(petrobras.PrecoMedio.Valor, 2),
            Is.EqualTo(101.90m));
    }

    [Test]
    public void CalcularPrecoMedio_SemTransacoes_DeveSerZero()
    {
        Assert.That(petrobras.PrecoMedio.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void VendaMaiorQueQuantidade_DeveLancarExcecaoEPreservarPosicao()
    {
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-10),
            100m,
            10);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-5),
            120m,
            15);

        Assert.Throws<InvalidOperationException>(() =>
            petrobras.AdicionarTransacao(
                TipoOperacaoEnum.Venda,
                dataReferencia,
                110m,
                30));

        // A venda rejeitada não deve alterar a posição.
        Assert.That(petrobras.Transacoes.Count, Is.EqualTo(2));
        Assert.That(petrobras.Quantidade, Is.EqualTo(25));
        Assert.That(petrobras.ValorInvestido.Valor, Is.EqualTo(2800m));
        Assert.That(petrobras.PrecoMedio.Valor, Is.EqualTo(112m));
    }

    [Test]
    public void CompraLancadaForaDeOrdemCronologica_DeveCalcularPrecoMedioCorretamente()
    {
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-2),
            80m,
            20);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-5),
            120m,
            15);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-10),
            100m,
            10);

        Assert.That(
            Math.Round(petrobras.PrecoMedio.Valor, 2),
            Is.EqualTo(97.78m));
    }

    [Test]
    public void VendaLancadaForaDeOrdemCronologica_DeveCalcularPrecoMedioCorretamente()
    {
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-10),
            100m,
            10);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-5),
            120m,
            15);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-2),
            80m,
            20);

        // Registrada antes da venda, mas ocorreu depois dela.
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(1),
            105m,
            20);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Venda,
            dataReferencia,
            115m,
            30);

        Assert.That(
            Math.Round(petrobras.PrecoMedio.Valor, 2),
            Is.EqualTo(101.90m));
    }
}