using CarteiraInvest.Application.Interfaces.Servicos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarteiraInvest.Infra.Identidade;

public sealed class ServicoToken : IServicoToken
{
    private readonly ConfiguracaoJwt _configuracaoJwt;

    public ServicoToken(IOptions<ConfiguracaoJwt> configuracaoJwt)
    {
        _configuracaoJwt = configuracaoJwt.Value;
    }

    public Task<string> GerarTokenAsync(string id, string? email, IList<string> perfis)
    {
        var agora = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, id),
            new (JwtRegisteredClaimNames.Email, email)
        };

        foreach (var perfil in perfis)
        {
            claims.Add(new Claim("role", perfil));
        }

        var chave = new SymmetricSecurityKey(Convert.FromBase64String(_configuracaoJwt.Chave));

        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracaoJwt.Issuer,
            audience: _configuracaoJwt.Audience,
            claims: claims,
            notBefore: agora,
            expires: agora.AddMinutes(_configuracaoJwt.ExpiracaoEmMinutos),
            signingCredentials: credenciais
        );

        var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenHandler);
    }
}
