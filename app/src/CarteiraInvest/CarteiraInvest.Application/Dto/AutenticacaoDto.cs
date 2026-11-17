using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarteiraInvest.Application.Dto;

public sealed record AutenticacaoDto
    (
    [Required(ErrorMessage = "O e-mail é obrigatório.")]string Email, 
    [Required(ErrorMessage = "A senha é obrigatória.")]  string Senha
    );
