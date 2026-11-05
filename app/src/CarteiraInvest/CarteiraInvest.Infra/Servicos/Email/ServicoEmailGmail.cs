using CarteiraInvest.Application.Interfaces.Servicos;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace CarteiraInvest.Infra.Servicos.Email;

public sealed class ServicoEmailGmail(IOptions<ConfiguracaoEmail> options) : IServicoEmail
{
    private readonly ConfiguracaoEmail _configuracao = options.Value;
    public async Task EnviarAsync(string destinatario, string assunto, string mensagem, CancellationToken cancellationToken)
    {
       var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_configuracao.NomeRemetente, _configuracao.EmailRemetente));

        email.To.Add(MailboxAddress.Parse(destinatario));
        email.Subject = assunto;
        email.Body = new TextPart("plain") { Text = mensagem };

        using var cliente = new SmtpClient();

        await cliente.ConnectAsync(
            _configuracao.ServidorSmtp,
            _configuracao.PortaSmtp,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await cliente.AuthenticateAsync(
            _configuracao.EmailRemetente,
            _configuracao.Senha,
            cancellationToken);

        await cliente.SendAsync(email, cancellationToken);
        await cliente.DisconnectAsync(true, cancellationToken);
    }
}
