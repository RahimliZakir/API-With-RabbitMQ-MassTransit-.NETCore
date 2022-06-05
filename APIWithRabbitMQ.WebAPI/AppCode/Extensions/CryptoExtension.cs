using System.Security.Cryptography;
using System.Text;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class Extension
    {
        const string secretKey = "@88m0v!3-sysT3M88@";

        public static string ToMD5(this string value)
        {
            using (MD5 provider = MD5.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);

                buffer = provider.ComputeHash(buffer);

                StringBuilder sb = new StringBuilder();

                foreach (byte item in buffer)
                {
                    sb.Append(item.ToString("x2"));
                }

                //return sb.ToString();

                return string.Join("", buffer.Select(b => b.ToString("x2")));
            }
        }

        public static string Encrypt(this string value, string key)
        {
            try
            {
                using (TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider())
                using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] keyBuffer = md5Provider.ComputeHash(Encoding.UTF8.GetBytes($"!{key}?"));
                    byte[] ivBuffer = md5Provider.ComputeHash(Encoding.UTF8.GetBytes($"@{key}#"));

                    ICryptoTransform transform = provider.CreateEncryptor(keyBuffer, ivBuffer);

                    using (MemoryStream ms = new MemoryStream())
                    using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(value);
                        cs.Write(buffer, 0, buffer.Length);
                        cs.FlushFinalBlock();

                        ms.Position = 0;
                        byte[] result = new byte[ms.Length];

                        ms.Read(result, 0, result.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string Encrypt(this string value)
        {
            return Encrypt(value, secretKey.ToMD5());
        }

        public static string Decrypt(this string value, string key)
        {
            try
            {
                using (TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider())
                using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] keyBuffer = md5Provider.ComputeHash(Encoding.UTF8.GetBytes($"!{key}?"));
                    byte[] ivBuffer = md5Provider.ComputeHash(Encoding.UTF8.GetBytes($"@{key}#"));

                    ICryptoTransform transform = provider.CreateDecryptor(keyBuffer, ivBuffer);

                    using (MemoryStream ms = new MemoryStream())
                    using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    {
                        byte[] buffer = Convert.FromBase64String(value);
                        cs.Write(buffer, 0, buffer.Length);
                        cs.FlushFinalBlock();

                        ms.Position = 0;
                        byte[] result = new byte[ms.Length];

                        ms.Read(result, 0, result.Length);

                        return Encoding.UTF8.GetString(result);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string Decrypt(this string value)
        {
            return Decrypt(value, secretKey.ToMD5());
        }
    }
}
