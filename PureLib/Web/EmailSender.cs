using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public sealed class EmailSender : IDisposable {
        private static readonly char[] _mailAddressSeparators = new char[] { ';', ',' };
        private MailMessage _message;
        private SmtpClient _client;

        public event SendCompletedEventHandler SendCompleted;

        public void SendMail(string host, int port, bool enableSsl, string userName, string password, string senderName, string from, string to,
            string subject, string body, bool isBodyHtml, bool sendAsync, string cc = null, string bcc = null) {

            _client = new SmtpClient(host);
            _client.Port = port;
            _client.EnableSsl = enableSsl;
            if (!userName.IsNullOrEmpty() && !password.IsNullOrEmpty())
                _client.Credentials = new NetworkCredential(userName, password);

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
                _client.SendCompleted += new SendCompletedEventHandler(ClientSendCompleted);
                _client.SendAsync(_message, null);
            }
            else {
                _client.Send(_message);
                Dispose();
            }
        }

        private void ClientSendCompleted(object sender, AsyncCompletedEventArgs e) {
            if (SendCompleted != null)
                SendCompleted(sender, e);
            Dispose();
        }

        private static void ParseMailAddress(string address, MailAddressCollection mac) {
            foreach (string s in address.Split(_mailAddressSeparators, StringSplitOptions.RemoveEmptyEntries)) {
                mac.Add(new MailAddress(s));
            }
        }

        public void Dispose() {
            _message.Dispose();
            _client.Dispose();
        }
    }
}
