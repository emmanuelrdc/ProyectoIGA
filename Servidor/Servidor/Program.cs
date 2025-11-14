using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // El proveedor de MySQL
// ... otros usings ...

namespace Servidor
{
    internal class Program
    {
        private const string MySqlConnectionString = "Server=localhost; Port=3306; Database=new_schema; Uid=root; Pwd=root1234;";

        // Clase que maneja la lógica principal del servidor TCP
        class TcpServer
        {
            //static void Main()
            public static async Task Main()
            {
                TcpListener server = null;
                int port = 13000;

                try
                {
                    server = new TcpListener(System.Net.IPAddress.Any, port);
                    server.Start();
                    Console.WriteLine($"Servidor iniciado en el puerto {port}. Esperando conexiones...");

                    while (true)
                    {
                        TcpClient client;
                        // 1. Aceptar Conexión del Cliente TCP
                        //TcpClient client = server.AcceptTcpClient();
                        client = await server.AcceptTcpClientAsync();
                        _ = Task.Run(() => HandleClientAsync(client));

                        Console.WriteLine("¡Cliente TCP conectado!");

                        // 2. Manejar la conexión del cliente en un método (más limpio)
                        //HandleClientAsync(client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error de Servidor TCP: {e.Message}");
                }
                finally
                {
                    server?.Stop();
                }
            }

            // Método para manejar la lógica de un cliente específico
            /*
            private static void HandleClient(TcpClient client)
            {
                using (client) // El 'using' asegura que el cliente se cierre
                {
                    NetworkStream stream = client.GetStream();

                    try
                    {
                        // 1. Leer Mensaje del Cliente
                        byte[] buffer = new byte[256];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        Console.WriteLine($"Recibido: {message}");

                        // 2. Lógica Clave: Usar la BD para responder o almacenar
                        string dbResponse = QueryDatabase(message);

                        // 3. Responder al Cliente
                        byte[] responseBytes = Encoding.UTF8.GetBytes(dbResponse);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error manejando cliente/BD: {ex.Message}");
                    }
                }
            }*/
           
            private static async Task HandleClientAsync(TcpClient client)
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesleidos = await stream.ReadAsync(buffer, 0, buffer.Length);

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesleidos).Trim();
                        Console.WriteLine($"Recibido: {message}");

                        string respuesta = await Database.QueryAsync(message);

                        byte[] responseBytes = Encoding.UTF8.GetBytes(respuesta);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

            }

            static class Database
            {
                private const string ConnString = "Server=localhost; Port=3306; Database=new_schema; Uid=root; Pwd=root1234;";

                public static async Task<string> QueryAsync(string name)
                {
                    var conn = new MySqlConnection(ConnString);

                    await conn.OpenAsync();

                    string sql = "SELECT nombre FROM usuarios WHERE nombre = @nombre";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@nombre", name);

                    var result = await cmd.ExecuteScalarAsync();

                    return result != null
                        ? $"Dato encontrado: {result}"
                        : $"No se encontró el dato '{name}'";
                }
            }
            /*
            // Nuevo método para interactuar con la base de datos
            private static string QueryDatabase(string receivedData)
            {
                // Solo se abre la conexión para la operación que se necesite
                using (MySqlConnection connection = new MySqlConnection(MySqlConnectionString))
                {
                    try
                    {
                        connection.Open();
                        // 💡 Aquí defines lo que hará la BD:
                        // Por ejemplo: si el mensaje contiene una consulta SQL, la ejecutas.
                        // O si es un dato, lo insertas.

                        // Ejemplo: Consulta la base de datos con un valor recibido
                        string sql = $"SELECT nombre FROM usuarios WHERE nombre = '{receivedData}'";

                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            object result = command.ExecuteScalar(); // Devuelve el primer valor

                            if (result != null)
                            {
                                return $"Dato encontrado: {result.ToString()}";
                            }
                            else
                            {
                                return $"Mensaje recibido '{receivedData}', pero no se encontraron datos.";
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        // Si falla la BD, el servidor debe manejarlo y responder al cliente
                        return $"Error de BD: {ex.Message}";
                    }
                }
            }
            */

        }
    }
}