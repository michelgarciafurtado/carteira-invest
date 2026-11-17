using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Servicos.Email;

public sealed class ConfiguracaoEmail
{
    public string ServidorSmtp { get; set; } = "smtp.gmail.com";
    public int PortaSmtp { get; set; } = 587;
    public string NomeRemetente { get; set; } = "Carteira Invest";
    public string EmailRemetente { get; set; } = string.Empty;
    public string SenhaApp { get; set; } = string.Empty;
}
