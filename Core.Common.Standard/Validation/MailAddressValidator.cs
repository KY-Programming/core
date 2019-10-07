using System;
using System.Linq;
using System.Net.Mail;
using KY.Core.Properties;

namespace KY.Core.Validation
{
    public class MailAddressValidator
    {
        public bool IsNullOrEmptyValid { get; set; }
        public string LastError { get; private set; }

        public MailAddressValidator(bool isNullOrEmptyValid = true)
        {
            this.IsNullOrEmptyValid = isNullOrEmptyValid;
        }

        public bool IsValid(string mail, bool? isNullOrEmptyValid = null)
        {
            try
            {
                if (string.IsNullOrEmpty(mail))
                    return isNullOrEmptyValid ?? this.IsNullOrEmptyValid;

                new MailAddress(mail);
                return true;
            }
            catch (Exception exception)
            {
                this.LastError = exception.Message;
                return false;
            }
        }

        public bool HasKnownError(string mail)
        {
            if (mail == null)
                return false;

            if (mail.EndsWith("@gmail.de"))
            {
                this.LastError = string.Format(Resources.MailAdressInvalidDomain, "@gmail.com");
                return true;
            }
            if (mail.EndsWith("@googlemail.de"))
            {
                this.LastError = string.Format(Resources.MailAdressInvalidDomain, "@googlemail.com");
                return true;
            }
            if (mail.EndsWith(".ru") || mail.EndsWith(".su") || mail.EndsWith(".ua") || mail.EndsWith(".by") || mail.EndsWith(".bg"))
            {
                this.LastError = Resources.MailAdressUntrustedTopLevelDomain;
                return true;
            }
            return false;
        }

        public bool IsBlocked(string mail)
        {
            if (mail == null)
                return false;

            string[] domains = { "@freenet." };
            return domains.Any(mail.Contains);
        }

        public bool IsBlacklisted(string mail)
        {
            if (mail == null)
                return false;

            string[] domains = { "@unit-gruppe.de", "mark357177@hotmail.com", "barny182@hotmail.com" };
            return domains.Any(mail.Contains);
        }
    }
}