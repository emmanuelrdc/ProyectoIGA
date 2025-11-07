using System;
using System.Net.Sockets;
using System.Text;

class ClienteTCP
{
    static void Main()
    {
        string server = "127.0.0.1";
        int port = 13000;

        Console.WriteLine("--- Cliente de Consulta (TCP) ---");
        Console.WriteLine($"Conectando a: {server}:{port}");

        // El bloque 'using' asegura que el cliente se cierre correctamente al salir del bloque
        using (TcpClient client = new TcpClient())
        {
            try
            {
                // 1. Conexión al Servidor
                client.Connect(server, port);
                Console.WriteLine("¡Conectado al servidor!");

                // El NetworkStream también debe cerrarse automáticamente
                using (NetworkStream stream = client.GetStream())
                {
                    // --- Bucle Interactivo para enviar múltiples comandos ---
                    while (true)
                    {
                        Console.Write("Escribe el ID a buscar o 'salir' para terminar: ");
                        string message = Console.ReadLine();

                        if (message?.ToLower() == "salir") break;
                        if (string.IsNullOrEmpty(message)) continue;

                        // 2. Enviar Datos (Comando)
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine($"-> Enviado: '{message}'");

                        // 3. Recibir Respuesta
                        byte[] buffer = new byte[512]; // Aumentamos el tamaño del buffer por si la respuesta de BD es larga
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        // Solo procesa si se leyeron datos
                        if (bytesRead > 0)
                        {
                            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"<- Recibido: {response}");
                        }
                        else
                        {
                            // Si bytesRead es 0, el servidor probablemente cerró la conexión
                            Console.WriteLine("El servidor cerró la conexión.");
                            break;
                        }

                        Console.WriteLine("------------------------------------------");
                    }
                }
            }
            catch (SocketException sex) when (sex.ErrorCode == 10061) // Error común de "conexión rechazada"
            {
                Console.WriteLine($"Error de Conexión: Asegúrate de que el servidor esté activo en {server}:{port}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Inesperado: {e.Message}");
            }
        }
        Console.WriteLine("Cliente desconectado.");
    }
}