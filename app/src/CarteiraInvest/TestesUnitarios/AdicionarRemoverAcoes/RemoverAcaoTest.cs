using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestesUnitarios.AdicionarRemoverAcoes;

[TestFixture]
public class RemoverAcaoTest
{
    AcaoCarteira _carteira;
    private AcaoCarteira Petrobras = null!;
    Guid IdInvestidor = new Guid();
    Guid IdCorretora = new Guid();
    
    [SetUp]
    public void SetUp()
    {
        Petrobras = new AcaoCarteira(
           "PETR4",
           IdInvestidor,
           IdCorretora);
    }

    [Test]
    public void AdicionarAcaoCateira_DevePassar()
    {
        _carteira.AdicionarTransacao(TipoOperacaoEnum.Compra,
                                    DateTime.Now.AddDays(-2),53.88m ,10);
        _carteira.AdicionarTransacao(TipoOperacaoEnum.Venda,
                                    DateTime.Now.AddDays(-1), 55.88m, 10);

    }
    [Test]
    public void AdicionarAcaoCarteira_DeveFalhar()
    {

    }
    [Test]
    public void RemoverAcaoCarteira_DevePassar()
    {

    }
    [Test]
    public void RemoverAcaoCarteira_DeveFalhar()
    {

    }
}
