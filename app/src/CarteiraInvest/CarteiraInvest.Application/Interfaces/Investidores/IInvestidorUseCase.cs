using CarteiraInvest.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Interfaces.Interfaces;

public interface IInvestidorUseCase
{
    Task<InvestidorDto> CriarInvestidorAsync(CriarInvestidorDto criarInvestidorDto);
}
