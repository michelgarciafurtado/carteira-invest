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
    [SetUp]
    public void Setup()
    {
        petrobras = new AcaoCarteira("PETR4", Guid.NewGuid(), Guid.NewGuid());
        compra1 = new AcaoTransacao("PETR4", new Guid(), TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-10), 100.0m, 10);
        compra2 = new AcaoTransacao("PETR4", new Guid(), TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-10), 120.0m, 15);
        compra3 = new AcaoTransacao("PETR4", new Guid(), TipoOperacaoEnum.Compra, DateTime.Now.AddDays(-2), 80.0m, 20);
        venda1 = new AcaoTransacao("PETR4", new Guid(), TipoOperacaoEnum.Venda, DateTime.Now, 115.0m, 30);
        Compra4 = new AcaoTransacao("PETR4", new Guid(), TipoOperacaoEnum.Compra, DateTime.Now.AddDays(1), 105.0m, 20);
    }

    [Test]
    public void CalculaPrecoMedio_DevePassar()
    {
        petrobras.AdicionarTransacao(compra1);
        petrobras.AdicionarTransacao(compra2);
        petrobras.AdicionarTransacao(compra3);
        petrobras.AdicionarTransacao(venda1);
        petrobras.AdicionarTransacao(Compra4);

        Assert.Equals(101.90m, petrobras.PrecoMedio);
    }
}
