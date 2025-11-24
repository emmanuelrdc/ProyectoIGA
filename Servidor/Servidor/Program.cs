using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Servidor
{
    internal class Program
    {
        class TcpServer
        {
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
                        client = await server.AcceptTcpClientAsync();
                        _ = Task.Run(() => HandleClientAsync(client));

                        Console.WriteLine("¡Cliente TCP conectado!");
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

                        string respuesta = await ProcessCommandAsync(message);

                        byte[] responseBytes = Encoding.UTF8.GetBytes(respuesta);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

            }

            private static async Task<string> ProcessCommandAsync(string message)
            {
                try
                {
                    string[] partes = message.Split('|');
                    if (partes.Length == 0)
                    {
                        return "Error|Mensaje vacio";
                    }
                    string comando = partes[0].ToUpper();

                    switch (comando)
                    {
                        case "LOGIN":
                            if (partes.Length < 3)
                                return "Error|Formato invalido";
                            return await Database.LoginAsync(partes[1], partes[2]);

                        case "REGISTER":
                            if (partes.Length < 3)
                                return "Error|Formato invalido";
                            return await Database.RegisterAsync(partes[1], partes[2]);

                        case "GETCHATS":
                            if (partes.Length < 2)
                                return "Error|Formato invalido";
                            return await Database.GetChatsAsync(int.Parse(partes[1]));

                        case "GETMESSAGES":
                            if (partes.Length < 2)
                                return "Error|Formato invalido";
                            return await Database.GetMessagesAsync(int.Parse(partes[1]));

                        case "SENDMESSAGE":
                            if (partes.Length < 4)
                                return "Error|Formato invalido";
                            // Restaurar las comas
                            string mensajeOriginal = partes[3].Replace("<<COMA>>", ",");
                            return await Database.SendMessageAsync(int.Parse(partes[1]), int.Parse(partes[2]), mensajeOriginal);

                        case "COUNTMESSAGES":
                            if (partes.Length < 2)
                                return "Error|Formato invalido";
                            return await Database.CountMessagesAsync(int.Parse(partes[1]));

                        default:
                            return "Error|Comando desconocido";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error|{ex.Message}";
                }
            }

            static class Database
            {
                private const string ConnString = "Server=localhost; Port=3306; Database=new_schema; Uid=root; Pwd=root1234;";

                public static async Task<string> LoginAsync(string usuario, string password)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = "SELECT id_usuario, nombre FROM usuarios WHERE nombre = @usuario AND contraseña = @password";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@usuario", usuario);
                                cmd.Parameters.AddWithValue("@password", password);

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        int idUsuario = reader.GetInt32(0);
                                        string nombreUsuario = reader.GetString(1);

                                        return $"Success|{idUsuario}|{nombreUsuario}";
                                    }
                                    else
                                    {
                                        return "Error|Usuario o contraseña incorrectos";
                                    }
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return $"Error|Error de BD: {ex.Message}";
                    }
                }

                public static async Task<string> RegisterAsync(string usuario, string password)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            // Verificar si existe
                            string queryVerificar = "SELECT COUNT(*) FROM usuarios WHERE nombre = @usuario";
                            using (var cmdVerificar = new MySqlCommand(queryVerificar, conn))
                            {
                                cmdVerificar.Parameters.AddWithValue("@usuario", usuario);

                                int existe = Convert.ToInt32(await cmdVerificar.ExecuteScalarAsync());

                                if (existe > 0)
                                {
                                    return "Error|Este usuario ya existe";
                                }
                            }

                            // Insertar usuario
                            string queryInsertar = "INSERT INTO usuarios (nombre, contraseña) VALUES (@usuario, @password)";
                            using (var cmdInsertar = new MySqlCommand(queryInsertar, conn))
                            {
                                cmdInsertar.Parameters.AddWithValue("@usuario", usuario);
                                cmdInsertar.Parameters.AddWithValue("@password", password);

                                await cmdInsertar.ExecuteNonQueryAsync();

                                return "Success|Usuario registrado exitosamente";
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return $"Error|Error de BD: {ex.Message}";
                    }
                }

                public static async Task<string> GetChatsAsync(int idUsuario)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = @"
                                SELECT c.id_chat, c.nombre_chat, c.es_individual,
                                       (SELECT u.nombre 
                                        FROM usuarios_chats uc2 
                                        JOIN usuarios u ON uc2.id_usuario = u.id_usuario
                                        WHERE uc2.id_chat = c.id_chat 
                                        AND uc2.id_usuario != @idUsuario
                                        LIMIT 1) as nombre_otro_usuario
                                FROM chats c
                                INNER JOIN usuarios_chats uc ON c.id_chat = uc.id_chat
                                WHERE uc.id_usuario = @idUsuario
                                ORDER BY c.id_chat DESC";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    string chats = "";

                                    while (await reader.ReadAsync())
                                    {
                                        int idChat = reader.GetInt32(0);
                                        bool esIndividual = reader.GetBoolean(2);
                                        string nombreChat;

                                        if (esIndividual)
                                        {
                                            nombreChat = reader.IsDBNull(3) ? "Usuario" : reader.GetString(3);
                                        }
                                        else
                                        {
                                            nombreChat = reader.IsDBNull(1) ? "Grupo sin nombre" : reader.GetString(1);
                                        }

                                        // Formato: idChat,nombreChat,esIndividual;
                                        chats += idChat + "," + nombreChat + "," + (esIndividual ? "1" : "0") + ";";
                                    }

                                    if (chats.Length > 0)
                                        chats = chats.TrimEnd(';');

                                    return "Success|" + chats;
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return "Error|Error de BD: " + ex.Message;
                    }
                }

                // NUEVO: Obtener mensajes de un chat
                public static async Task<string> GetMessagesAsync(int idChat)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = @"
                                SELECT m.mensaje, m.fecha, u.nombre
                                FROM mensajes m
                                JOIN usuarios u ON m.id_usuario = u.id_usuario
                                WHERE m.id_chat = @idChat
                                ORDER BY m.fecha ASC";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@idChat", idChat);

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    string mensajes = "";

                                    while (await reader.ReadAsync())
                                    {
                                        string nombre = reader.GetString(2);
                                        string mensaje = reader.GetString(0);
                                        DateTime fecha = reader.GetDateTime(1);

                                        // Reemplazar comas por otro carácter para no romper el protocolo
                                        nombre = nombre.Replace(",", "");
                                        mensaje = mensaje.Replace(",", "<<COMA>>");

                                        // Formato: nombre,mensaje,fecha;
                                        mensajes += nombre + "," + mensaje + "," + fecha.ToString("HH:mm") + ";";
                                    }

                                    if (mensajes.Length > 0)
                                        mensajes = mensajes.TrimEnd(';');

                                    return "Success|" + mensajes;
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return "Error|Error de BD: " + ex.Message;
                    }
                }

                // NUEVO: Enviar mensaje
                public static async Task<string> SendMessageAsync(int idChat, int idUsuario, string mensaje)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = @"
                                INSERT INTO mensajes (id_chat, id_usuario, mensaje, fecha)
                                VALUES (@idChat, @idUsuario, @mensaje, NOW())";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@idChat", idChat);
                                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                                cmd.Parameters.AddWithValue("@mensaje", mensaje);

                                await cmd.ExecuteNonQueryAsync();

                                return "Success|Mensaje enviado";
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return "Error|Error de BD: " + ex.Message;
                    }
                }

                // NUEVO: Contar mensajes de un chat
                public static async Task<string> CountMessagesAsync(int idChat)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = "SELECT COUNT(*) FROM mensajes WHERE id_chat = @idChat";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@idChat", idChat);

                                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                                return "Success|" + count;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return "Error|Error de BD: " + ex.Message;
                    }
                }
            }
        }
    }
}

/*using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Servidor
{
    internal class Program
    {
        class TcpServer
        {
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

                        string respuesta = await ProcessCommandAsync(message);

                        byte[] responseBytes = Encoding.UTF8.GetBytes(respuesta);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

            }

            private static async Task<string> ProcessCommandAsync(string message)
            {
                try
                {
                    string[] partes = message.Split('|');
                    if (partes.Length == 0)
                    {
                        return "Error, mensaje vacio";
                    }
                    string comando = partes[0].ToUpper();

                    switch (comando)
                    {
                        case "Login":
                            if (partes.Length < 3)
                                return "Error, formato invalido";
                            return await Database.LoginAsync(partes[1], partes[2]);

                        case "Register":
                            if (partes.Length < 3)
                                return "Error, formato invalido";
                            return await Database.RegisterAsync(partes[1], partes[2]);

                        default:
                            return $"Error, comando desconocido";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error {ex.Message}";
                }
            }

            static class Database
            {
                // CAMBIA ESTOS DATOS por los de tu BD
                private const string ConnString = "Server=localhost; Port=3306; Database=chat_app; Uid=root; Pwd=root;";

                // NUEVO: Login de usuario
                public static async Task<string> LoginAsync(string usuario, string password)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            string sql = "SELECT id_usuario, nombre FROM usuarios WHERE nombre = @usuario AND contraseña = @password";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@usuario", usuario);
                                cmd.Parameters.AddWithValue("@password", password);

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        int idUsuario = reader.GetInt32("id_usuario");
                                        string nombreUsuario = reader.GetString("nombre");

                                        return $"Success|{idUsuario}|{nombreUsuario}";
                                    }
                                    else
                                    {
                                        return "ERROR|Usuario o contraseña incorrectos";
                                    }
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return $"Error, Error de BD: {ex.Message}";
                    }
                }

                public static async Task<string> RegisterAsync(string usuario, string password)
                {
                    try
                    {
                        using (var conn = new MySqlConnection(ConnString))
                        {
                            await conn.OpenAsync();

                            // Verificar si existe
                            string queryVerificar = "SELECT COUNT(*) FROM usuarios WHERE nombre = @usuario";
                            using (var cmdVerificar = new MySqlCommand(queryVerificar, conn))
                            {
                                cmdVerificar.Parameters.AddWithValue("@usuario", usuario);

                                int existe = Convert.ToInt32(await cmdVerificar.ExecuteScalarAsync());

                                if (existe > 0)
                                {
                                    return "Error, Este usuario ya existe";
                                }
                            }

                            // Insertar usuario
                            string queryInsertar = "INSERT INTO usuarios (nombre, contraseña) VALUES (@usuario, @password)";
                            using (var cmdInsertar = new MySqlCommand(queryInsertar, conn))
                            {
                                cmdInsertar.Parameters.AddWithValue("@usuario", usuario);
                                cmdInsertar.Parameters.AddWithValue("@password", password);

                                await cmdInsertar.ExecuteNonQueryAsync();

                                return "Success, Usuario registrado exitosamente";
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        return $"Error, Error de BD: {ex.Message}";
                    }
                }
            }
        }
    }
}*/