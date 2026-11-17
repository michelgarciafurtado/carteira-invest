using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public sealed record CotacaoDto
(string Symbol, decimal Preco);


public sealed record SymbolDto
    (string Symbol, decimal Preco, string ShortName);
