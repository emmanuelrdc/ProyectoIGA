using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace chat
{
    public partial class menuChat : Form
    {
        private int idUsuarioActual;
        private string nombreUsuarioActual;
        
        public menuChat(int idUsuario, string nombreUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
            nombreUsuarioActual = nombreUsuario;
            label2.Text = $"Hola, {nombreUsuarioActual}";
            richTextBox1.Visible = false;
            panel7.Visible = true;
        }

        private Form chatActual;

        private void Form2_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
            CargarChats();
        }
        private void CargarChats()
        {
            
            try
            {
                foreach (Control ctrl in panelChats.Controls) {
                    if (ctrl != panel4 && ctrl != panel2)
                    {
                        ctrl.Dispose();
                    }
                }
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT c.id_chat, c.nombre_chat, c.es_individual,
                               (SELECT u.nombre 
                                FROM usuarios_chats uc2 
                                JOIN usuarios u ON uc2.id_usuario = u.id_usuario
                                WHERE uc2.id_chat = c.id_chat 
                                AND uc2.id_usuario != @idUsuario
                                LIMIT 1) as nombre_otro_usuario
                        FROM chats c
                        INNER JOIN usuarios_chats uc ON c.id_chat = uc.id_chat
                        WHERE uc.id_usuario = @idUsuario
                        ORDER BY c.id_chat DESC"; MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuarioActual);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    int yPosition = 0;

                    while (reader.Read())
                    {
                        int idChat = reader.GetInt32("id_chat");
                        bool esIndividual = reader.GetBoolean("es_individual");
                        string nombreChat;

                        if (esIndividual)
                        {
                            // Para chats individuales, mostrar el nombre del otro usuario
                            nombreChat = reader.IsDBNull(reader.GetOrdinal("nombre_otro_usuario"))
                                ? "Usuario"
                                : reader.GetString("nombre_otro_usuario");
                        }
                        else
                        {
                            // Para chats grupales, mostrar el nombre del grupo
                            nombreChat = reader.IsDBNull(reader.GetOrdinal("nombre_chat"))
                                ? "Grupo sin nombre"
                                : reader.GetString("nombre_chat");
                        }

                        // Crear el control del chat
                        ChatItem chatItem = new ChatItem(idChat, idUsuarioActual, nombreChat, esIndividual);
                        chatItem.Location = new System.Drawing.Point(panel4.Width + 5, yPosition);
                        chatItem.Width = panelChats.Width - panel4.Width - 25;
                        chatItem.ChatClicked += ChatItem_ChatClicked;

                        panelChats.Controls.Add(chatItem);
                        yPosition += chatItem.Height + 5;
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar chats: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
        }
        private void ChatItem_ChatClicked(object sender, EventArgs e)
        {
            ChatItem chatItem = (ChatItem)sender;
            // Aquí cargarás los mensajes del chat seleccionado
            MessageBox.Show($"Chat seleccionado: {chatItem.NombreChat}\nID: {chatItem.IdChat}",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // TODO: Implementar la carga de mensajes en panel3
           
            using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT m.fecha, m.mensaje, u.nombre AS nombre_usuario FROM mensajes m JOIN usuarios u ON m.id_usuario = u.id_usuario WHERE m.id_chat = @is_chat ORDER BY m.fecha ASC";
                    using (MySqlCommand comm = new MySqlCommand(query, conn))
                    {
                        comm.Parameters.AddWithValue("@id_chat", chatItem.IdChat);
                        using (MySqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nom = reader["nombre_usuario"].ToString();
                                string msj = reader["mensaje"].ToString();
                                DateTime fecha = reader.GetDateTime("fecha");
                                string msjcom = $"[{fecha}] {nom}: {msj}";
                                richTextBox2.AppendText(msjcom);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error al leer los mensajes: " + ex.Message);
                }
            }
            panel7.Visible = false;
            richTextBox1.Visible = true;
            richTextBox2.ReadOnly = true;
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormAgregarUsuario formAgregar = new FormAgregarUsuario(idUsuarioActual);

            if (formAgregar.ShowDialog() == DialogResult.OK)
            {
                int idUsuarioSeleccionado = formAgregar.IdUsuarioSeleccionado;
                string nombreUsuarioSeleccionado = formAgregar.NombreUsuarioSeleccionado;

                // Verificar si ya existe un chat individual con este usuario
                int? idChatExistente = VerificarChatIndividualExistente(idUsuarioSeleccionado);

                if (idChatExistente.HasValue)
                {
                    MessageBox.Show($"Ya tienes un chat con {nombreUsuarioSeleccionado}",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar chats para asegurarse de que esté visible
                    CargarChats();
                }
                else
                {
                    // Crear nuevo chat individual
                    CrearChatIndividual(idUsuarioSeleccionado, nombreUsuarioSeleccionado);
                }
            }
        }
        

        private void logOut_Button_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro de que quieres cerrar sesión?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Cerrar este formulario y mostrar el login nuevamente
                this.Close();

                // Mostrar el Form1 (login) nuevamente
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Form1)
                    {
                        form.Show();
                        return;
                    }
                }

                // Si no existe, crear uno nuevo
                Form1 loginForm = new Form1();
                loginForm.Show();
            }
        }
        private void CrearChatIndividual(int idOtroUsuario, string nombreOtroUsuario)
        {
            try
            {
                // USAR DatabaseConnection.ConnectionString
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 1. Crear el chat
                        string queryChat = "INSERT INTO chats (nombre_chat, es_individual) VALUES (NULL, 1)";
                        MySqlCommand cmdChat = new MySqlCommand(queryChat, conn, transaction);
                        cmdChat.ExecuteNonQuery();

                        // Obtener el ID del chat recién creado
                        int idNuevoChat = (int)cmdChat.LastInsertedId;

                        // 2. Agregar ambos usuarios al chat
                        string queryUsuariosChats = "INSERT INTO usuarios_chats (id_usuario, id_chat) VALUES (@idUsuario, @idChat)";

                        // Agregar usuario actual
                        MySqlCommand cmdUsuario1 = new MySqlCommand(queryUsuariosChats, conn, transaction);
                        cmdUsuario1.Parameters.AddWithValue("@idUsuario", idUsuarioActual);
                        cmdUsuario1.Parameters.AddWithValue("@idChat", idNuevoChat);
                        cmdUsuario1.ExecuteNonQuery();

                        // Agregar otro usuario
                        MySqlCommand cmdUsuario2 = new MySqlCommand(queryUsuariosChats, conn, transaction);
                        cmdUsuario2.Parameters.AddWithValue("@idUsuario", idOtroUsuario);
                        cmdUsuario2.Parameters.AddWithValue("@idChat", idNuevoChat);
                        cmdUsuario2.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show($"Chat con {nombreOtroUsuario} creado exitosamente",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Recargar la lista de chats
                        CargarChats();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear chat: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int? VerificarChatIndividualExistente(int idOtroUsuario)
        {
            try
            {
                // USAR DatabaseConnection.ConnectionString
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    // Buscar chat individual que contenga ambos usuarios
                    string query = @"
                        SELECT c.id_chat
                        FROM chats c
                        WHERE c.es_individual = 1
                        AND EXISTS (
                            SELECT 1 FROM usuarios_chats uc1 
                            WHERE uc1.id_chat = c.id_chat AND uc1.id_usuario = @idUsuario1
                        )
                        AND EXISTS (
                            SELECT 1 FROM usuarios_chats uc2 
                            WHERE uc2.id_chat = c.id_chat AND uc2.id_usuario = @idUsuario2
                        )";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idUsuario1", idUsuarioActual);
                    cmd.Parameters.AddWithValue("@idUsuario2", idOtroUsuario);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar chat existente: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            label3.Text = nombreUsuarioActual.ToString();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        private int idChatActual = -1;

        private void CargarMensajes(int idChat)
        {
            try
            {
                richTextBox2.Clear();

                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    string query = @"SELECT m.mensaje, m.fecha, u.nombre
                             FROM mensajes m
                             INNER JOIN usuarios u ON m.id_usuario = u.id_usuario
                             WHERE m.id_chat = @idChat
                             ORDER BY m.fecha ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idChat", idChat);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nombre = reader.GetString("nombre");
                                string mensaje = reader.GetString("mensaje");
                                DateTime fecha = reader.GetDateTime("fecha");

                                // Mostrar el mensaje en el RichTextBox
                                richTextBox2.AppendText($"[{fecha:HH:mm}] {nombre}: {mensaje}\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mensajes: " + ex.Message);
            }
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            string mensaje = richTextBox1.Text.Trim();
            if (string.IsNullOrEmpty(mensaje))
            {
                MessageBox.Show("No puedes enviar un mensaje vacío.");
                return;
            }

            if (idChatActual == -1)
            {
                MessageBox.Show("Selecciona un chat primero.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO mensajes (id_chat, id_usuario, mensaje, fecha)
                             VALUES (@idChat, @idUsuario, @mensaje, NOW())";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idChat", idChatActual);
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuarioActual);
                        cmd.Parameters.AddWithValue("@mensaje", mensaje);
                        cmd.ExecuteNonQuery();
                    }
                }

                richTextBox2.AppendText($"[{DateTime.Now:HH:mm}] {nombreUsuarioActual}: {mensaje}\n");
                richTextBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar mensaje: " + ex.Message);
            }
        }
        

    }
}
