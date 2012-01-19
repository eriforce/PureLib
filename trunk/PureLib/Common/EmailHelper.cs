using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PureLib.Common {
    public static class EmailHelper {
        private const string htmlTemplateToken = "%%";
        private static readonly char[] mailAddressSeparators = new char[] { ';', ',' };
        private static MailMessage _message;

        public static event SendCompletedEventHandler SendCompleted;

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

        public static void SendMail(string host, int port, bool enableSsl, string userName, string password, string senderName, string from, string to,
            string subject, string body, bool isBodyHtml, bool sendAsync, string cc = null, string bcc = null) {

            using (SmtpClient client = new SmtpClient(host)) {
                client.Port = port;
                client.EnableSsl = enableSsl;
                if (!userName.IsNullOrEmpty() && !password.IsNullOrEmpty())
                    client.Credentials = new NetworkCredential(userName, password);

                _message = new MailMessage();
                _message.From = new MailAddress(from, senderName.IsNullOrEmpty() ? from : senderName);
                ParseMailAddress(to, _message.To);
                if (!cc.IsNullOrEmpty())
                    ParseMailAddress(cc, _message.CC);
                if (!bcc.IsNullOrEmpty())
                    ParseMailAddress(bcc, _message.Bcc);
                _message.SubjectEncoding = Encoding.UTF8;
                _message.BodyEncoding = Encoding.UTF8;
                _message.IsBodyHtml = isBodyHtml;
                _message.Subject = subject;
                _message.Body = body;

                if (sendAsync) {
                    client.SendCompleted += new SendCompletedEventHandler(ClientSendCompleted);
                    client.SendAsync(_message, null);
                }
                else {
                    client.Send(_message);
                    _message.Dispose();
                }
            }
        }

        private static void ClientSendCompleted(object sender, AsyncCompletedEventArgs e) {
            if (SendCompleted != null)
                SendCompleted(sender, e);
            _message.Dispose();
        }

        private static void ParseMailAddress(string address, MailAddressCollection mac) {
            foreach (string s in address.Split(mailAddressSeparators, StringSplitOptions.RemoveEmptyEntries)) {
                mac.Add(new MailAddress(s));
            }
        }
    }
}
