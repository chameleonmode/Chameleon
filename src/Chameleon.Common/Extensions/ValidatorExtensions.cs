using System.Net.Mail;

namespace Chameleon.Common.Extensions
{
    public static class ValidatorExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
