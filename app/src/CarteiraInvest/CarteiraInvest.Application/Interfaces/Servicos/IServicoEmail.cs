using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Application.Interfaces.Servicos;

public interface IServicoEmail
{
    Task EnviarAsync(string destinatario, string assunto, string mensagem, CancellationToken cancellationToken);
}
