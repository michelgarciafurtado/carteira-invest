using CarteiraInvest.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Interfaces.Identidade;

public interface IServicoIdentidade
{
    Task<ResultadoCriacaoUsuario> CriarUsuarioAsync(string email, string senha);
    Task EnviarConfirmacaoCadastroAsync(string usuarioId);
    Task<bool> ConfirmarCadastroAsync(string usuarioId, string codigo);
    Task<UsuarioLogadoDto> AutenticarUsuario(string email, string senha);
}
