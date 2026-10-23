using System;
using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using NUnit.Framework;

namespace TestesUnitarios;

public class CalcularValorAtualTest
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
    public void DeveCalcularValorAtualComoZero_QuandoNaoHouverCotacaoNemTransacoes()
    {
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(0m));
    }

    [Test]
    public void DeveCalcularValorAtualCorretamente_AposAdicionarTransacoesEAtualizarCotacao()
    {
        // Arrange
        petrobras.AdicionarTransacao(compra1); // 10
        petrobras.AdicionarTransacao(compra2); // +15 = 25
        petrobras.AdicionarTransacao(compra3); // +20 = 45
        petrobras.AdicionarTransacao(venda1);  // -30 = 15 ações em custódia

        // Act
        petrobras.AtualizarCotacao(DateTimeOffset.UtcNow, PrecoAtual);

        // Assert: 15 ações * R$ 59.53 = R$ 892.95
        decimal valorEsperado = 15 * PrecoAtual;
        Assert.That(petrobras.ValorAtual.Valor, Is.EqualTo(valorEsperado));
    }
}