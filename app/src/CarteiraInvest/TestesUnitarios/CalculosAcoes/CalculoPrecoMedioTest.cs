using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;

namespace TestesUnitarios.CalculosAcoes;

public class CalculoPrecoMedioTest
{
    AcaoCarteira petrobras;
    AcaoTransacao compra1;
    AcaoTransacao compra2;
    AcaoTransacao compra3;
    AcaoTransacao venda1;
    AcaoTransacao Compra4;
    Guid investidorId = Guid.NewGuid();
    Guid corretoraId = Guid.NewGuid();
    [SetUp]
    public void Setup()
    {
        petrobras = new AcaoCarteira("PETR4", investidorId, corretoraId);
        compra1 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-10), 100.0m, 10);
        compra2 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-5), 120.0m, 15);
        compra3 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-2), 80.0m, 20);
        venda1 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Venda, DateTime.Now, 115.0m, 30);
        Compra4 = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Compra, DateTime.Now.AddDays(1), 105.0m, 20);
    }

    [Test]
    public void CalcularPrecoMedio_DevePassar()
    {
        petrobras.AdicionarTransacao(compra1);
        petrobras.AdicionarTransacao(compra2);
        petrobras.AdicionarTransacao(compra3);
        petrobras.AdicionarTransacao(venda1);
        petrobras.AdicionarTransacao(Compra4);

        Assert.That(Math.Round(petrobras.PrecoMedio.Valor, 2), Is.EqualTo(101.90m));
    }

    [Test]
    public void CalcularPrecoMedio_SemTransacoes_DeveSerZero()
    {
        Assert.That(petrobras.PrecoMedio.Valor, Is.EqualTo(0.0m));
    }

    [Test]
    public void VendaMaiorQueQuantidade_DeveLancarExcecao()
    {
        petrobras.AdicionarTransacao(compra1);
        petrobras.AdicionarTransacao(compra2);
        var vendaInvalida = new AcaoTransacao("PETR4", investidorId, TipoOperacaoEnum.Venda, DateTime.Now, 110.0m, 30);
        Assert.Throws<InvalidOperationException>(() => petrobras.AdicionarTransacao(vendaInvalida));
        Assert.That(petrobras.Transacoes.Count, Is.EqualTo(2));//confirma que a venda inválida não foi adicionada
    }
    [Test]
    public void CompraLancadaForaDeOrdemCronologica_DevePassar()
    {
        petrobras.AdicionarTransacao(compra3);
        petrobras.AdicionarTransacao(compra2);
        petrobras.AdicionarTransacao(compra1);
        Assert.That(Math.Round(petrobras.PrecoMedio.Valor, 2), Is.EqualTo(97.78m));
    }

    [Test]
    public void VendaLancadaForaDeOrdemCronologica_DevePassar()
    {
        petrobras.AdicionarTransacao(compra1);
        petrobras.AdicionarTransacao(compra2);
        petrobras.AdicionarTransacao(compra3);
        petrobras.AdicionarTransacao(Compra4);
        petrobras.AdicionarTransacao(venda1);
        Assert.That(Math.Round(petrobras.PrecoMedio.Valor, 2), Is.EqualTo(101.9m));
    }
}
