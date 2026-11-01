using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto.InvestodoresDtos;

public sealed record InvestidorCriadoDto
(
     Guid InvestidorId,
    string IdentityUserId,
    string Nome,
    string Sobrenome
);
