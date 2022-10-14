using System.Security.Authentication;
using System.Text;
using MongoDB.Driver;
using System.Security.Cryptography;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services;

public static class CoreService
{
    public static string GetNewGuid()
    {
        return Guid.NewGuid().ToString();
    }

    public static MongoClient GetMongoClient(string connectionString)
    {
        var settings = MongoClientSettings.FromUrl(
            new MongoUrl(connectionString)
        );
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        return new MongoClient(settings);
    }

    public static IMongoDatabase GetDatabase(IDatabaseSettings databaseSettings)
    {
        return GetMongoClient(databaseSettings.ConnectionString).GetDatabase(databaseSettings.Database);
    }
    
    public static string Encrypt(string encryptString, string key = "secretKey")
    {
        var clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using var encryptor = Aes.Create();
        var pdb = new Rfc2898DeriveBytes(key, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
        }
        encryptString = Convert.ToBase64String(ms.ToArray());

        return encryptString;
    }

    public static string Decrypt(string cipherText, string key = "secretKey")
    {
        cipherText = cipherText.Replace(" ", "+");
        var cipherBytes = Convert.FromBase64String(cipherText);
        using var encryptor = Aes.Create();
        var pdb = new Rfc2898DeriveBytes(key, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();
        }
        cipherText = Encoding.Unicode.GetString(ms.ToArray());

        return cipherText;
    }
}