using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VI_Home1
{
    public class MvcApplication : System.Web.HttpApplication // Отправка сообщения(о возникшей ошибке) на почту при помощи библиотеки mailkit (порт 465) т.к. с помощью библиотеки System.Net.Mail не получалась отправка
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error(object sender, EventArgs e)
        {

            HttpException lastErrorWrapper =
                Server.GetLastError() as HttpException;

            Exception lastError = lastErrorWrapper;
            if (lastErrorWrapper.InnerException != null)
                lastError = lastErrorWrapper.InnerException;

            string lastErrorTypeName = lastError.GetType().ToString();
            string lastErrorMessage = lastError.Message;
            string lastErrorStackTrace = lastError.StackTrace;

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Моя компания", "electromast@mail.ru"));
            message.To.Add(new MailboxAddress("1993zhandarm@gmail.com"));
            message.Subject = "Сообщение от Микулича Егора";
            message.Body = new BodyBuilder()
            {
                HtmlBody = string.Format(@"
            <html>
            <body>
              <h1>An Error Has Occurred!</h1>
              <table cellpadding=""5"" cellspacing=""0"" border=""1"">
              <tr>
              <tdtext-align: right;font-weight: bold"">URL:</td>
              <td>{0}</td>
              </tr>
              <tr>
              <tdtext-align: right;font-weight: bold"">User:</td>
              <td>{1}</td>
              </tr>
              <tr>
              <tdtext-align: right;font-weight: bold"">Exception Type:</td>
              <td>{2}</td>
              </tr>
              <tr>
              <tdtext-align: right;font-weight: bold"">Message:</td>
              <td>{3}</td>
              </tr>
              <tr>
              <tdtext-align: right;font-weight: bold"">Stack Trace:</td>
              <td>{4}</td>
              </tr> 
              </table>
            </body>
            </html>",
                Request.RawUrl,
                User.Identity.Name,
                lastErrorTypeName,
                lastErrorMessage,
                lastErrorStackTrace.Replace(Environment.NewLine, "<br />"))
            }.ToMessageBody();

            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("1993zhandarm@gmail.com", "Здесь ввести пароль");
                client.Send(message);

                client.Disconnect(true);

            }
        }

    }
}