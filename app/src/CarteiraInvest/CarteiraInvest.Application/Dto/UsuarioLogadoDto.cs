using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public sealed record UsuarioLogadoDto
(
    string UsuarioId,
    string token
);
