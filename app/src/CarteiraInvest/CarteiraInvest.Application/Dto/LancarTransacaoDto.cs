using CarteiraInvest.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public sealed record LancarTransacaoDto
(
    [Required(ErrorMessage ="Id do Investidor nao identificado")]
    Guid InvestidorId,
    [Required(ErrorMessage = "Id da Corretora inexistente")]
    Guid CorrretoraId,
    [Required(ErrorMessage = "Código da ação não encontrado")]
    string Symbol,
    [Required(ErrorMessage = "Informe a quantidade de ações compradas")]
    int qtd,
    [Required(ErrorMessage = "Informe o valor unitario da ação comprada")]
    decimal precoUnitario,
    [Required(ErrorMessage = "Informe a data da operação")]
    DateTime DataOperacao,
    [Required(ErrorMessage = "Informe o tipo de operação compra ou venda")]
    TipoOperacaoEnum TipoOperacao
    );
