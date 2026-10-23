using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios;

public class CalcularLucroPrejuizoTest
{
    AcaoCarteira petrobras;
    AcaoTransacao compra1;
    AcaoTransacao compra2;
    AcaoTransacao compra3;
    AcaoTransacao venda1;
    AcaoTransacao Compra4;
    Guid investidorId = Guid.NewGuid();
    Guid corretoraId = Guid.NewGuid();
    decimal PrecoAtual = 59.53m;

    [SetUp]
    public void Setup()
    {
        petrobras = new AcaoCarteira("PETR4", investidorId, corretoraId);
        compra1 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-10), 57.83m, 10);
        compra2 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-5), 57.36m, 15);
        compra3 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-2), 58.01m, 20);
        venda1 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Venda, DateTime.Now, 59.42m, 30);
        Compra4 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(1), 59.61m, 20);
    }

    [Test]
    public void DeveRetornarLucroPrejuizoZero_QuandoNaoHouverValorInvestidoOuValorAtual()
    {
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, PrecoAtual);

        Assert.That(petrobras.LucroPrejuizo.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void DeveCalcularLucroCorretamente_QuandoValorAtualForMaiorQueValorInvestido()
    {
        // Arrange
        petrobras.AdicionarTransacao(compra1);
        petrobras.AdicionarTransacao(compra2);
        petrobras.AdicionarTransacao(compra3);
        petrobras.AdicionarTransacao(venda1); // Sobram 15 ações a Preço Médio R$ 57.754

        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, PrecoAtual); // 59.53

        // Assert: ValorAtual - ValorInvestido
        decimal lucroEsperado = petrobras.ValorAtual.Valor - petrobras.ValorInvestido.Valor;
        Assert.That(petrobras.LucroPrejuizo.Valor, Is.EqualTo(lucroEsperado));
    }

    [Test]
    public void DeveCalcularPrejuizoCorretamente_QuandoPrecoAtualForMenorQuePrecoMedio()
    {
        // Arrange
        petrobras.AdicionarTransacao(compra1);
        decimal precoComPrejuizo = 50.00m;

        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, precoComPrejuizo);

        // Assert: (10 * 50.00) - (10 * 57.83) = 500 - 578.30 = -78.30
        Assert.That(petrobras.LucroPrejuizo.Valor, Is.EqualTo(-78.30m));
    }
}