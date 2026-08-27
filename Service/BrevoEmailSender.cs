using System.Text.Encodings.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace login.Services;

public class BrevoEmailSender : IEmailSender<IdentityUser>
{
    private readonly IConfiguration _configuration;

    public BrevoEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendConfirmationLinkAsync(
        IdentityUser user,
        string email,
        string confirmationLink)
    {
        var subject = "Confirme o seu endereço de e-mail";

        var htmlMessage = $"""
            <html>
            <body>
                <h2>Confirme o seu e-mail</h2>

                <p>Olá,</p>

                <p>
                    Obrigado por criar uma conta.
                    Clique no botão abaixo para confirmar o seu endereço de e-mail.
                </p>

                <p>
                    <a href="{HtmlEncoder.Default.Encode(confirmationLink)}"
                       style="
                       display:inline-block;
                       padding:12px 20px;
                       background:#2563eb;
                       color:white;
                       text-decoration:none;
                       border-radius:6px;">
                        Confirmar e-mail
                    </a>
                </p>

                <p>
                    Se não criou esta conta, pode ignorar esta mensagem.
                </p>
            </body>
            </html>
            """;

        await SendEmailAsync(email, subject, htmlMessage);
    }

    public async Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string email,
        string resetLink)
    {
        var subject = "Redefinição da sua senha";

        var htmlMessage = $"""
            <html>
            <body>
                <h2>Redefinir senha</h2>

                <p>
                    Recebemos um pedido para redefinir a sua senha.
                </p>

                <p>
                    Clique no botão abaixo para criar uma nova senha.
                </p>

                <p>
                    <a href="{HtmlEncoder.Default.Encode(resetLink)}"
                       style="
                       display:inline-block;
                       padding:12px 20px;
                       background:#2563eb;
                       color:white;
                       text-decoration:none;
                       border-radius:6px;">
                        Redefinir senha
                    </a>
                </p>

                <p>
                    Se não fez este pedido, ignore este e-mail.
                </p>
            </body>
            </html>
            """;

        await SendEmailAsync(email, subject, htmlMessage);
    }

    public async Task SendPasswordResetCodeAsync(
        IdentityUser user,
        string email,
        string resetCode)
    {
        var subject = "Código para redefinir a sua senha";

        var htmlMessage = $"""
            <html>
            <body>
                <h2>Redefinir senha</h2>

                <p>
                    Recebemos um pedido para redefinir a sua senha.
                </p>

                <p>O seu código de redefinição é:</p>

                <p style="
                    font-size:24px;
                    font-weight:bold;
                    letter-spacing:4px;">
                    {HtmlEncoder.Default.Encode(resetCode)}
                </p>

                <p>
                    Se não fez este pedido, ignore este e-mail.
                </p>
            </body>
            </html>
            """;

        await SendEmailAsync(email, subject, htmlMessage);
    }

    private async Task SendEmailAsync(
        string recipient,
        string subject,
        string htmlMessage)
    {
        var host = _configuration["Brevo:SmtpHost"]
            ?? throw new InvalidOperationException(
                "Brevo:SmtpHost não configurado.");

        if (!int.TryParse(
                _configuration["Brevo:SmtpPort"],
                out var port))
        {
            port = 587;
        }

        var username = _configuration["Brevo:SmtpUsername"]
            ?? throw new InvalidOperationException(
                "Brevo:SmtpUsername não configurado.");

        var password = _configuration["Brevo:SmtpPassword"]
            ?? throw new InvalidOperationException(
                "Brevo:SmtpPassword não configurado.");

        var fromEmail = _configuration["Brevo:FromEmail"]
            ?? throw new InvalidOperationException(
                "Brevo:FromEmail não configurado.");

        var fromName = _configuration["Brevo:FromName"]
            ?? "Login";

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(fromName, fromEmail));

        message.To.Add(
            MailboxAddress.Parse(recipient));

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            host,
            port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            username,
            password);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}