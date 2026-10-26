using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios;

public class CalcularLucroPrejuizoTest
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
    public void DeveRetornarLucroPrejuizoZero_QuandoNaoHouverTransacoes()
    {
        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, 59.53m);

        // Assert
        Assert.That(petrobras.LucroPrejuizo.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void DeveCalcularLucroCorretamente_QuandoValorAtualForMaiorQueValorInvestido()
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
        // Compras: R$ 2.598,90 para 45 ações.
        // Após vender 30: restam 15 ações com custo de R$ 866,30.
        // Valor atual: 15 × R$ 59,53 = R$ 892,95.
        // Lucro não realizado: R$ 892,95 − R$ 866,30 = R$ 26,65.
        Assert.That(
            petrobras.LucroPrejuizo.Valor,
            Is.EqualTo(26.65m).Within(0.000001m));
    }

    [Test]
    public void DeveCalcularPrejuizoCorretamente_QuandoPrecoAtualForMenorQuePrecoMedio()
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
        // Valor atual: R$ 500,00.
        // Valor investido: R$ 578,30.
        // Prejuízo: −R$ 78,30.
        Assert.That(
            petrobras.LucroPrejuizo.Valor,
            Is.EqualTo(-78.30m));
    }
}