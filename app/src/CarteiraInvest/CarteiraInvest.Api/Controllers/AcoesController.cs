using CarteiraInvest.Application.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarteiraInvest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcoesController : ControllerBase
    {
        private readonly IServicoCotacao _servicoCotacao;
        public AcoesController(IServicoCotacao servicoCotacao)
        {
            _servicoCotacao = servicoCotacao;
        }
        [HttpGet("cotacao")]
        public async Task<IActionResult> GetCotacaoAsync([FromQuery] string symbol)
        {
            var cotacao = await _servicoCotacao.ObterCotacaoAsync(symbol);
            return Ok(new { Cotacao = cotacao });
        }
        [HttpGet("acao")]
        public async Task<IActionResult> GetSymbolAsync([FromQuery] string symbol)
        {
            var acao = await _servicoCotacao.ObterSymbolAsync(symbol);
            return Ok(new { Acao = acao });
        }
    }
}
