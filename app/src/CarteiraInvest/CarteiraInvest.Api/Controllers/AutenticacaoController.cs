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
}
