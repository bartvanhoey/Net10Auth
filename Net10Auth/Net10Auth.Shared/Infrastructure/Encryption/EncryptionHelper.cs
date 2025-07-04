using System.Security.Cryptography;
using System.Text;

namespace Net10Auth.Shared.Infrastructure.Encryption;

public static class EncryptionHelper
{
    public static string Encrypt(string plainText, string secretKey, string initializationVector)
    {
        using var aes = Aes.Create();
        try
        {
            aes.Key = Encoding.UTF8.GetBytes(secretKey);
            aes.IV = Encoding.UTF8.GetBytes(initializationVector);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs)) sw.Write(plainText);
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText, string secretKey, string initializationVector)
    {
        using var aes = Aes.Create();
        try
        {
            aes.Key = Encoding.UTF8.GetBytes(secretKey);
            aes.IV = Encoding.UTF8.GetBytes(initializationVector);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}

