using CarteiraInvest.Application.Dto;
using CarteiraInvest.Application.Interfaces.Identidade;
using CarteiraInvest.Application.Interfaces.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CarteiraInvest.Api.Controllers;

[Route("carteirainvest/api/[controller]")]
[ApiController]
public class InvestidorController(IInvestidorUseCase criarInvestidorUseCase) : ControllerBase
{
    private readonly IInvestidorUseCase _criarInvestidorUseCase = criarInvestidorUseCase;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Hello, World!" });
    }

    [HttpPost]
    public async Task<IActionResult> CriarinvestidorAsync([FromBody] CriarInvestidorDto dto)
    {
        var resultado = await _criarInvestidorUseCase.CriarInvestidorAsync(dto);

       
        return StatusCode(StatusCodes.Status201Created, new
        {
            Mensagem = "Usuario criado com sucesso",
            UsuarioId = resultado.IdendityUser
        });
    }
    [HttpPost("confirmar-cadastro")]
    public async Task<IActionResult> ConfirmarCadastroAsync([FromBody] ConfirmaCadastroDto dto)
    {
        var resultado = await _criarInvestidorUseCase.ConfirmarCadastroAsync(dto.UsuarioId, dto.Codigo);
        if (resultado)
        {
            return Ok(new { Mensagem = "Cadastro confirmado com sucesso" });
        }
        else
        {
            return BadRequest(new { Mensagem = "Falha ao confirmar cadastro" });
        }
    }

}
