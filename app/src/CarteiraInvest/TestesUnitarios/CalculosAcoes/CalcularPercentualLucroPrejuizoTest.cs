using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios;

public class CalcularPercentualLucroPrejuizoTest
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
    public void DeveRetornarZeroPorCento_QuandoNaoHouverValorInvestido()
    {
        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, 59.53m);

        // Assert
        Assert.That(petrobras.PercentualLucroPrejuizo, Is.EqualTo(0m));
    }

    [Test]
    public void DeveCalcularPercentualDeLucroCorretamente_QuandoHouverGanhos()
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

        // Assert
        // Custo da posição restante: R$ 866,30.
        // Valor atual: R$ 892,95.
        // Lucro: R$ 26,65.
        decimal percentualEsperado = (26.65m / 866.30m) * 100m;

        Assert.That(
            petrobras.PercentualLucroPrejuizo,
            Is.EqualTo(percentualEsperado).Within(0.0001m));
    }

    [Test]
    public void DeveCalcularPercentualDePrejuizoCorretamente_QuandoHouverPerdas()
    {
        // Arrange
        petrobras.AdicionarTransacao(
            TipoOperacaoEnum.Compra,
            dataReferencia,
            57.83m,
            10);

        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, 50.00m);

        // Assert
        // Valor investido: R$ 578,30.
        // Valor atual: R$ 500,00.
        // Prejuízo: −R$ 78,30.
        decimal percentualEsperado = (-78.30m / 578.30m) * 100m;

        Assert.That(
            petrobras.PercentualLucroPrejuizo,
            Is.EqualTo(percentualEsperado).Within(0.0001m));
    }
}