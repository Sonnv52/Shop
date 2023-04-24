using ForgotPasswordService.Message;
using ForgotPasswordService.Model;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ForgotPasswordService.Repository
{
    public class EmailRepository : ISendMailService<TokenResetMessage>
    {
        private readonly EmailCofiuration emailCofiuration;
        public EmailRepository(EmailCofiuration email) => emailCofiuration = email;
        
        public async Task SendAsync(TokenResetMessage message)
        {
            var emailMessage = await Task.Run(() => CreateEmailMessage(message));
            await Task.Run(() =>  Send(emailMessage));

        }
        private MimeMessage CreateEmailMessage(TokenResetMessage message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("email", emailCofiuration.From));
            email.To.AddRange(message.To);
            email.Subject = message.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };
            return email;
        }
        private void Send(MimeMessage mail)
        {
            using var clinet = new SmtpClient();
            try
            {
                clinet.ServerCertificateValidationCallback = (s, c, h, e) => true;
                clinet.Connect(emailCofiuration.SmtpServer, emailCofiuration.Port, SecureSocketOptions.SslOnConnect);
                clinet.AuthenticationMechanisms.Remove("XOAUTH2");
                clinet.Authenticate(emailCofiuration.UserName, emailCofiuration.Password);
                clinet.Send(mail);
            }catch
            {
                throw;
            }
            finally {
                clinet.Disconnect(true);
                clinet.Dispose(); 
            }
        }
    }
}
