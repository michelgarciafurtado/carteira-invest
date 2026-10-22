using CarteiraInvest.Domain.Entities;
using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestesUnitarios.CalculosAcoes
{
    public class CalculoQuantidadeTotalTest
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
            int quantidade,
            int dias,
            decimal preco = 100m)
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
        public void SemTransacoes_QuantidadeDeveSerZero()
        {
            _carteira.RecalcularPosicao();

            Assert.That(_carteira.Quantidade, Is.EqualTo(0));
        }

        [Test]
        public void Compras_DevemSomarQuantidades()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 10, 0));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 15, 1));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 20, 2));

            Assert.That(_carteira.Quantidade, Is.EqualTo(45));
        }

        [Test]
        public void VendaParcial_DeveReduzirQuantidade()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 45, 0));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Venda, 30, 1));

            Assert.That(_carteira.Quantidade, Is.EqualTo(15));
        }

        [Test]
        public void VendaTotal_DeveZerarQuantidade()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 10, 0));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Venda, 10, 1));

            Assert.That(_carteira.Quantidade, Is.EqualTo(0));
        }

        [Test]
        public void CompraAposVendaTotal_DeveAtualizarQuantidade()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 10, 0));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Venda, 10, 1));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 7, 2));

            Assert.That(_carteira.Quantidade, Is.EqualTo(7));
        }

        [Test]
        public void VendaInseridaForaDeOrdem_DeveConsiderarDataDaOperacao()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 10, 0));

            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 20, 2));

            // Inserida por último, mas realizada entre as compras.
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Venda, 5, 1));

            Assert.That(_carteira.Quantidade, Is.EqualTo(25));
        }

        [Test]
        public void VendaMaiorQueSaldo_DevePreservarQuantidadeETransacoes()
        {
            _carteira.AdicionarTransacao(
                CriarTransacao(TipoOperacaoEnum.Compra, 10, 0));

            var vendaInvalida =
                CriarTransacao(TipoOperacaoEnum.Venda, 11, 1);

            Assert.Throws<InvalidOperationException>(
                () => _carteira.AdicionarTransacao(vendaInvalida));

            Assert.That(_carteira.Quantidade, Is.EqualTo(10));
            Assert.That(_carteira.Transacoes.Count, Is.EqualTo(1));
        }

        [Test]
        public void VendaSemSaldo_DeveLancarExcecao()
        {
            var venda =
                CriarTransacao(TipoOperacaoEnum.Venda, 1, 0);

            Assert.Throws<InvalidOperationException>(
                () => _carteira.AdicionarTransacao(venda));

            Assert.That(_carteira.Quantidade, Is.EqualTo(0));
            Assert.That(_carteira.Transacoes, Is.Empty);
        }
    }
}
