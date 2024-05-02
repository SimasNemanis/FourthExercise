using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("App3 started.");
        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Receive information from App2");
            Console.WriteLine("2. Perform digital signature verification");
            Console.WriteLine("0. Exit");

            int choice = GetChoice();

            switch (choice)
            {
                case 1:
                    ReceiveInfoFromApp2();
                    break;
                case 2:
                    PerformDigitalSignatureVerification();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static int GetChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        return choice;
    }

    static void ReceiveInfoFromApp2()
    {
        TcpListener server = null;
        try
        {
            int port = 13001;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine("App3 listening for incoming connections from App2...");

            TcpClient client = server.AcceptTcpClient();
            using (NetworkStream stream = client.GetStream())
            {
                string directoryPath = "C:\\Users\\rolan\\source\\repos\\ThirdlevelRSA3\\ThirdlevelRSA3\\";
                string filePath = Path.Combine(directoryPath, "received_data.txt");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                Console.WriteLine($"Saving received data to: {filePath}");
                ReceiveFile(stream, filePath);

                string[] data = File.ReadAllLines(filePath);
                if (data.Length < 3)
                {
                    Console.WriteLine("Received data is incomplete.");
                    return;
                }

                string message = data[0];
                string publicKey = data[1];
                byte[] signature = Convert.FromBase64String(data[2]);

                Console.WriteLine("Received information from App2:");
                Console.WriteLine($"Message: {message}");
                Console.WriteLine($"Public Key: {publicKey}");
                Console.WriteLine($"Signature: {Convert.ToBase64String(signature)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving information from App2: {ex.Message}");
        }
        finally
        {
            server?.Stop();
        }
    }



    static void ReceiveFile(NetworkStream stream, string filePath)
    {
        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }
            Console.WriteLine("File received successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving file: {ex.Message}");
        }
    }

    static void PerformDigitalSignatureVerification()
    {
        try
        {
            string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdlevelRSA3\\ThirdlevelRSA3\\received_data.txt";

            string[] data = File.ReadAllLines(filePath);

            if (data.Length < 3)
            {
                Console.WriteLine("Received data is incomplete.");
                return;
            }

            string message = data[0];
            string publicKey = data[1];
            byte[] signature = Convert.FromBase64String(data[2]);

            bool isSignatureValid = VerifySignature(message, publicKey, signature);
            Console.WriteLine($"Digital signature verification result: {isSignatureValid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error performing digital signature verification: {ex.Message}");
        }
    }


    static bool VerifySignature(string message, string publicKey, byte[] signature)
    {
        try
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                byte[] data = Encoding.UTF8.GetBytes(message);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying digital signature: {ex.Message}");
            return false;
        }
    }

}
