using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Apply digital signature to text");
            Console.WriteLine("2. Send public key, message, and digital signature to another application");
            Console.WriteLine("0. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ApplyDigitalSignature();
                    break;
                case "2":
                    SendDataOverSocket();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }

    static void ApplyDigitalSignature()
    {
        Console.WriteLine("Enter the text to sign:");
        string textToSign = Console.ReadLine();

        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            string publicKey = rsa.ToXmlString(false);

            byte[] signature = SignText(textToSign, rsa.ExportParameters(true));

            string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA1\\ThirdLevelRSA1\\data.txt";
            WriteToFile(textToSign, publicKey, signature, filePath);

            Console.WriteLine("Digital signature applied to text.");
        }
    }

    static byte[] SignText(string text, RSAParameters privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            byte[] data = Encoding.UTF8.GetBytes(text);
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    static void WriteToFile(string text, string publicKey, byte[] signature, string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(text);
                writer.WriteLine(publicKey);
                writer.WriteLine(Convert.ToBase64String(signature));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing data to file: {ex.Message}");
        }
    }

    static void SendDataOverSocket()
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 13000))
            using (NetworkStream stream = client.GetStream())
            using (FileStream fileStream = File.OpenRead("C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA1\\ThirdLevelRSA1\\data.txt"))
            {
                fileStream.CopyTo(stream);
            }

            Console.WriteLine("Data sent successfully to another application over socket.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data over socket: {ex.Message}");
        }
    }
}
