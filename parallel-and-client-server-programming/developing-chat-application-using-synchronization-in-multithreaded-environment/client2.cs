using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

class Client
{
    const int PORT = 12345;
    const int BUFFER_SIZE = 1024;

    static void SendFile(NetworkStream stream, string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine("ERROR: File not found");
            return;
        }

        string command = "PUT " + Path.GetFileName(filename);
        byte[] commandBytes = Encoding.UTF8.GetBytes(command);
        stream.Write(commandBytes, 0, commandBytes.Length);

        byte[] buffer = new byte[BUFFER_SIZE];
        using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            int bytesRead;
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }
        }

        buffer = new byte[BUFFER_SIZE];
        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine("Server response: " + Encoding.UTF8.GetString(buffer, 0, bytesReceived));
    }

    static void DownloadFile(NetworkStream stream, string filename)
    {
        string command = "GET " + filename;
        byte[] commandBytes = Encoding.UTF8.GetBytes(command);
        stream.Write(commandBytes, 0, commandBytes.Length);

        byte[] buffer = new byte[BUFFER_SIZE];
        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

        if (response.StartsWith("ERROR"))
        {
            Console.WriteLine("Server response: " + response);
            return;
        }

        using (FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            do
            {
                file.Write(buffer, 0, bytesReceived);
                bytesReceived = stream.Read(buffer, 0, buffer.Length);
            } while (bytesReceived > 0);
        }

        Console.WriteLine("File downloaded successfully: " + filename);
    }

    static void Main()
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", PORT))
            using (NetworkStream stream = client.GetStream())
            {
                Console.WriteLine("Connected to server!");

                while (true)
                {
                    Console.Write("Enter command (LIST, PUT <file>, GET <file>, DELETE <file>, INFO <file>, EXIT): ");
                    string command = Console.ReadLine();

                    if (command == "EXIT") break;

                    if (command.StartsWith("PUT "))
                    {
                        string filename = command.Substring(4);
                        SendFile(stream, filename);
                    }
                    else if (command.StartsWith("GET "))
                    {
                        string filename = command.Substring(4);
                        DownloadFile(stream, filename);
                    }
                    else
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(command);
                        stream.Write(buffer, 0, buffer.Length);

                        buffer = new byte[BUFFER_SIZE];
                        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                        Console.WriteLine("Server response: " + Encoding.UTF8.GetString(buffer, 0, bytesReceived));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
        }
    }
}
