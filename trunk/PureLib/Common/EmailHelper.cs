using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to send email.
    /// </summary>
    public static class EmailHelper {
        private static readonly char[] mailAddressSeparators = new char[] { ';', ',' };
        private static MailMessage message;

        /// <summary>
        /// Represents the token string in HTML template.
        /// </summary>
        public const string htmlTemplateToken = "%%";

        /// <summary>
        /// Occurs when an asynchronous e-mail send operation completes.
        /// </summary>
        public static event SendCompletedEventHandler SendCompleted;

        /// <summary>
        /// Gets content from HTML template.
        /// </summary>
        /// <param name="htmlTemplate"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static string GetContentFromHtmlTemplate(string htmlTemplate, Dictionary<string, string> tokens) {
            string[] parts = htmlTemplate.Split(new string[] { htmlTemplateToken }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            foreach (string part in parts) {
                string value;
                if (tokens.TryGetValue(part, out value))
                    sb.Append(value);
                else
                    sb.Append(part);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sends a email.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="enableSsl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="senderName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="sendAsync"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        public static void SendMail(string host, int port, bool enableSsl, string userName, string password, string senderName, string from, string to,
            string subject, string body, bool isBodyHtml, bool sendAsync, string cc = null, string bcc = null) {

            using (SmtpClient client = new SmtpClient(host)) {
                client.Port = port;
                client.EnableSsl = enableSsl;
                if (!userName.IsNullOrEmpty() && !password.IsNullOrEmpty())
                    client.Credentials = new NetworkCredential(userName, password);

                message = new MailMessage();
                message.From = new MailAddress(from, senderName.IsNullOrEmpty() ? from : senderName);
                ParseMailAddress(to, message.To);
                if (!cc.IsNullOrEmpty())
                    ParseMailAddress(cc, message.CC);
                if (!bcc.IsNullOrEmpty())
                    ParseMailAddress(bcc, message.Bcc);
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = isBodyHtml;
                message.Subject = subject;
                message.Body = body;

                if (sendAsync) {
                    client.SendCompleted += new SendCompletedEventHandler(ClientSendCompleted);
                    client.SendAsync(message, null);
                }
                else {
                    client.Send(message);
                    message.Dispose();
                }
            }
        }

        private static void ClientSendCompleted(object sender, AsyncCompletedEventArgs e) {
            if (SendCompleted != null)
                SendCompleted(sender, e);
            message.Dispose();
        }

        private static void ParseMailAddress(string address, MailAddressCollection mac) {
            foreach (string s in address.Split(mailAddressSeparators, StringSplitOptions.RemoveEmptyEntries)) {
                mac.Add(new MailAddress(s));
            }
        }
    }
}
