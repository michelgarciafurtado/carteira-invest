using CarteiraInvest.Application.Dto;
using CarteiraInvest.Application.Interfaces.Identidade;
using Microsoft.AspNetCore.Mvc;

namespace CarteiraInvest.Api.Controllers;

[Route("carteirainvest/api/[controller]")]
[ApiController]
public class AutenticacaoController : ControllerBase
{
    private readonly IServicoIdentidade _servicoIdentidade;

    public AutenticacaoController(IServicoIdentidade servicoIdentidade)
    {
        _servicoIdentidade = servicoIdentidade;
    }

    [HttpPost]
    public async Task<IActionResult> AutenticarUsuario([FromBody] AutenticacaoDto autenticacaoDto)
    {
        if (autenticacaoDto == null || string.IsNullOrWhiteSpace(autenticacaoDto.Email) || string.IsNullOrWhiteSpace(autenticacaoDto.Senha))
        {
            return BadRequest("E-mail e senha são obrigatórios.");
        }
        var usuarioLogado = await _servicoIdentidade.AutenticarUsuario(autenticacaoDto.Email, autenticacaoDto.Senha);
        if (usuarioLogado == null)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }
        return Ok(usuarioLogado);
    }
}
