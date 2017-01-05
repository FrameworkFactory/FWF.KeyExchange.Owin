
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FWF.KeyExchange.Owin
{
    public static class StringExtensions
    {

        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        [DebuggerStepThrough]
        public static bool EqualsIgnoreCase(this string str, string compareTo)
        {
            if (string.IsNullOrEmpty(str))
            {
                return (string.IsNullOrEmpty(compareTo));
            }

            return str.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsValidGuid(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var newGuid = Guid.Empty;

            try
            {
                newGuid = new Guid(input);
            }
            catch (FormatException)
            {
                // Ignore
            }

            return (newGuid != Guid.Empty);
        }

        [DebuggerStepThrough]
        public static bool IsBase64(this string base64String)
        {
            // Credit: oybek http://stackoverflow.com/users/794764/oybek
            if (IsMissing(base64String)
                || base64String.Length%4 != 0
                || base64String.Contains(" ")
                || base64String.Contains("\t")
                || base64String.Contains("\r")
                || base64String.Contains("\n")
                )
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception)
            {
                // gulp
            }

            return false;
        }

        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (url.IsPresent())
            {
                if (!url.EndsWith("/"))
                {
                    return url + "/";
                }
            }

            return url;
        }
        
        
        public static string ReplaceOnce(this string input, string find, string replacement)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (find == null)
            {
                throw new ArgumentNullException("find");
            }
            if (replacement == null)
            {
                replacement = string.Empty;
            }

            var intBeginLocation = input.IndexOf(find, StringComparison.CurrentCultureIgnoreCase);

            if (intBeginLocation < 0)
            {
                return input;
            }

            var objBuilder = new StringBuilder(input.Length + replacement.Length);

            objBuilder.Append(input.Substring(0, intBeginLocation));

            objBuilder.Append(replacement);

            objBuilder.Append(input.Substring(intBeginLocation + find.Length));

            return objBuilder.ToString();
        }

        public static string RemoveInvalidFileNameChars(this string input)
        {
            // TODO: string replacement over and over again should be addressed

            foreach (var badChar in _invalidFileNameChars)
            {
                input = input.Replace(badChar.ToString(), string.Empty);
            }

            return input;
        }

        public static string RemoveInvalidPathChars(this string input)
        {
            // TODO: string replacement over and over again should be addressed

            foreach (var badChar in _invalidFileNameChars)
            {
                input = input.Replace(badChar.ToString(), string.Empty);
            }

            return input;
        }

        public static string Sha256(this string input)
        {
            if (input.IsMissing())
            {
                return string.Empty;
            }
            string result;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha.ComputeHash(bytes);
                result = Convert.ToBase64String(hash);
            }
            return result;
        }

        public static string Sha512(this string input)
        {
            if (input.IsMissing())
            {
                return string.Empty;
            }
            string result;
            using (SHA512 sha = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha.ComputeHash(bytes);
                result = Convert.ToBase64String(hash);
            }
            return result;
        }

    }
}
