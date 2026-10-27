using CarteiraInvest.Application.Dto;
using CarteiraInvest.Application.Interfaces.Identidade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CarteiraInvest.Api.Controllers;

[Route("carteirainvest/api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IServicoIdentidade _servicoIdentidade;

    public UsersController(IServicoIdentidade servicoIdentidade)
    {
        _servicoIdentidade = servicoIdentidade;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Hello, World!" });
    }

    [HttpPost]
    public async Task<IActionResult> CriarUsuarioAsync([FromBody] CriarUsuarioDto dto)
    {
        var resultado = await _servicoIdentidade.CriarUsuarioAsync(dto.Email, dto.Senha);

        if (!resultado.Sucesso)
            return BadRequest(new
            {
                Mensagem = "Não foi possivel criar o usuário",
                Erros = resultado.Erros
            });
        return StatusCode(StatusCodes.Status201Created, new
        {
            Mensagem = "Usuario criado com sucesso",
            UsuarioId = resultado.UsuarioId
        });
    }
}
