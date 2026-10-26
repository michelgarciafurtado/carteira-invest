using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios;

public class CalcularValorAtualTest
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

        dataReferencia = DateTime.UtcNow.AddDays(-1);
    }

    [Test]
    public void DeveRetornarValorAtualZero_QuandoNaoHouverCotacaoNemTransacoes()
    {
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void DeveRetornarValorAtualZero_QuandoHouverTransacaoSemCotacao()
    {
        // Arrange / Act
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia,
            57.83m,
            10);

        // Assert
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void DeveCalcularValorAtualCorretamente_AposAdicionarTransacoesEAtualizarCotacao()
    {
        // Arrange
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-10),
            57.83m,
            10);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-5),
            57.36m,
            15);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia.AddDays(-2),
            58.01m,
            20);

        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Venda,
            dataReferencia,
            59.42m,
            30);

        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, 59.53m);

        // Assert: 15 ações × R$ 59,53 = R$ 892,95.
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(892.95m));
    }

    [Test]
    public void DeveRecalcularValorAtual_AoAdicionarTransacaoComCotacaoExistente()
    {
        // Arrange
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, 59.53m);

        // Act
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia,
            57.83m,
            10);

        // Assert: 10 ações × R$ 59,53 = R$ 595,30.
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(595.30m));
    }
}