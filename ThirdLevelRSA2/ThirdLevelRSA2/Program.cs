using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Receive information from App1");
            Console.WriteLine("2. Change digital signature");
            Console.WriteLine("3. Send modified signature to App3");
            Console.WriteLine("0. Exit");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ReceiveInfoFromApp1();
                    break;
                case "2":
                    ChangeSignature();
                    break;
                case "3":
                    SendModifiedSignatureToApp3();
                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void ReceiveInfoFromApp1()
    {
        TcpListener server = null;
        try
        {
            int port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine("App2 listening for incoming connections from App1...");

            TcpClient client = server.AcceptTcpClient();
            using (NetworkStream stream = client.GetStream())
            {
                string directoryPath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA2\\ThirdLevelRSA2\\";
                string filePath = Path.Combine(directoryPath, "update.txt");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                Console.WriteLine($"Saving received data to: {filePath}");
                ReceiveFile(stream, filePath);

                string[] data = File.ReadAllLines(filePath);
                string message = data[0];
                string publicKey = data[1];
                byte[] signature = Convert.FromBase64String(data[2]);

                Console.WriteLine("Received information from App1:");
                Console.WriteLine($"Message: {message}");
                Console.WriteLine($"Public Key: {publicKey}");
                Console.WriteLine($"Signature: {Convert.ToBase64String(signature)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving information from App1: {ex.Message}");
        }
        finally
        {
            server?.Stop();
        }
    }




    static void SaveReceivedData(string message, string publicKey, byte[] signature)
    {
        try
        {
            string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA2\\ThirdLevelRSA2\\update.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(message);
                writer.WriteLine(publicKey);
                writer.WriteLine(Convert.ToBase64String(signature));
            }
            Console.WriteLine("Received data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving received data: {ex.Message}");
        }
    }

    static void ChangeSignature()
    {
        try
        {
            string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA2\\ThirdLevelRSA2\\update.txt";
            string[] data = File.ReadAllLines(filePath);
            string message = data[0];
            string publicKey = data[1];
            byte[] originalSignature = Convert.FromBase64String(data[2]);

            byte[] newSignature = ChangeSignature(originalSignature);

            SaveModifiedSignature(message, publicKey, newSignature);

            Console.WriteLine("Digital signature changed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing digital signature: {ex.Message}");
        }
    }

    static byte[] ChangeSignature(byte[] originalSignature)
    {
        byte[] newSignature = new byte[originalSignature.Length + 1];
        Array.Copy(originalSignature, newSignature, originalSignature.Length);
        newSignature[originalSignature.Length] = 0x01;
        return newSignature;
    }

    static void SaveModifiedSignature(string message, string publicKey, byte[] signature)
    {
        try
        {
            string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA2\\ThirdLevelRSA2\\update.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(message);
                writer.WriteLine(publicKey);
                writer.WriteLine(Convert.ToBase64String(signature));
            }
            Console.WriteLine("Modified signature saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving modified signature: {ex.Message}");
        }
    }

    static void SendModifiedSignatureToApp3()
    {
        try
        {
            int port = 13001; 
            IPAddress serverAddr = IPAddress.Parse("127.0.0.1");

            using (TcpClient client = new TcpClient(serverAddr.ToString(), port))
            using (NetworkStream stream = client.GetStream())
            {
                string filePath = "C:\\Users\\rolan\\source\\repos\\ThirdLevelRSA2\\ThirdLevelRSA2\\update.txt";
                SendFile(stream, filePath);
                Console.WriteLine("Modified signature sent to App3 successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending modified signature to App3: {ex.Message}");
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

    static void SendFile(NetworkStream stream, string filePath)
    {
        try
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                fileStream.CopyTo(stream);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending file: {ex.Message}");
        }
    }
}
