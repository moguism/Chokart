using System.Net;
using System.Net.Mail;
using System.Text;

namespace server.Services;

// https://blog.elmah.io/how-to-send-emails-from-csharp-net-the-definitive-tutorial/

public class EmailService
{
    private readonly string HOST = Environment.GetEnvironmentVariable("EmailHost");
    private readonly int PORT = int.Parse(Environment.GetEnvironmentVariable("EmailPort"));
    private readonly string ADDRESS = Environment.GetEnvironmentVariable("EmailAddress");
    private readonly string PASSWORD = Environment.GetEnvironmentVariable("EmailPassword");

    private async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        using SmtpClient client = new SmtpClient(HOST, PORT)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(ADDRESS, PASSWORD)
        };

        MailMessage mail = new MailMessage(ADDRESS, to, subject, body)
        {
            IsBodyHtml = isHtml,
        };

        await client.SendMailAsync(mail);

        Console.WriteLine("Email enviado");
    }

    public async Task CreateEmailUser(string email, int id, string code)
    {
        string to = email;
        string subject = "Verifica tu cuenta";

#if DEBUG
        string verificationUrl = $"http://localhost:4200/verify/{id}/{code}";
#else
        string verificationUrl = $"https://playchokart.com/verify/{id}/{code}";
#endif

        StringBuilder body = new StringBuilder();

        body.AppendLine("<html><body>");
        body.AppendLine("<p>Hola,</p>");
        body.AppendLine("<p>Haz clic en el botón de abajo para verificar tu cuenta:</p>");
        body.AppendLine($"<a href='{verificationUrl}' style='display:inline-block;padding:10px 20px;background-color:#4CAF50;color:white;text-decoration:none;border-radius:5px;'>Verificar Cuenta</a>");
        body.AppendLine("<p>Si no fuiste tú, puedes ignorar este correo.</p>");
        body.AppendLine("</body></html>");

        await SendEmailAsync(to, subject, body.ToString(), true);
    }
}