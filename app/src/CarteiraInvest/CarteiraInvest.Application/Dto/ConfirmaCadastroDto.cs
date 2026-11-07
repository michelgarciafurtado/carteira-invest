using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public sealed record ConfirmaCadastroDto(
    [Required] string UsuarioId, 
    [Required] string Codigo);

