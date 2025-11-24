using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace chat
{
    public partial class menuChat : Form
    {
        private int idUsuarioActual;
        private string nombreUsuarioActual;
        private int idChatActual = -1;
        private bool reemplazando = false;
        private System.Windows.Forms.Timer timerActualizar;
        private int ultimoNumeroMensajes = 0;

        /*
        private void InsertarTextoConEmojis(RichTextBox destino, string texto)
        {
            int pos = 0;
            while (pos < texto.Length)
            {
                bool encontrado = false;

                // Buscar emojis ordenados por longitud (más largos primero)
                foreach (var kvp in emojis.OrderByDescending(x => x.Key.Length))
                {
                    if (pos + kvp.Key.Length <= texto.Length &&
                        texto.Substring(pos, kvp.Key.Length) == kvp.Key)
                    {
                        InsertarImagenEnChat(destino, kvp.Value);
                        pos += kvp.Key.Length;
                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado)
                {
                    destino.AppendText(texto[pos].ToString());
                    pos++;
                }
            }
        }
       
        private Dictionary<string, Image> emojis = new Dictionary<string, Image>()
        {
            { ":)", Properties.Resources.sonrisa},
            { ":(", Properties.Resources.llorando},
            { "<3", Properties.Resources.corazon}
        };*/


        public menuChat(int idUsuario, string nombreUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
            nombreUsuarioActual = nombreUsuario;
            label2.Text = $"Hola, {nombreUsuarioActual}";

            // Configurar visibilidad inicial
            richTextBox1.Visible = false;
            panel7.Visible = true;
            richTextBox2.ReadOnly = true;

            // Configurar Timer para actualización automática
            timerActualizar = new System.Windows.Forms.Timer();
            timerActualizar.Interval = 2000; // 2 segundos
            timerActualizar.Tick += TimerActualizar_Tick;
            timerActualizar.Start();

            // Agregar evento KeyPress para optimizar reemplazo de emojis
            richTextBox1.KeyPress += RichTextBox1_KeyPress;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
            CargarChats();
        }
        /*
        private void ReemplazarEmojis()
        {
            int cursorPos = richTextBox1.SelectionStart;

            foreach (var kvp in emojis)
            {
                string textoEmoji = kvp.Key;
                Image imagenEmoji = kvp.Value;

                // Buscar desde el principio
                int indice = 0;
                while (indice < richTextBox1.Text.Length)
                {
                    indice = richTextBox1.Find(textoEmoji, indice, RichTextBoxFinds.None);

                    if (indice == -1) break;

                    richTextBox1.Select(indice, textoEmoji.Length);
                    InsertarImagenEnRichTextBox(imagenEmoji);

                    // Ajustar el índice después del reemplazo
                    indice = richTextBox1.SelectionStart;
                }
            }

            // Restaurar posición del cursor
            richTextBox1.Select(Math.Min(cursorPos, richTextBox1.TextLength), 0);
        }
        */
        private void Label4_Click(object sender, EventArgs e)
        {
            if (idChatActual == -1)
            {
                MessageBox.Show("Por favor selecciona un chat primero", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Obtener información del chat actual
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT c.nombre_chat, c.es_individual
                FROM chats c
                WHERE c.id_chat = @idChat";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idChat", idChatActual);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        bool esIndividual = reader.GetBoolean("es_individual");
                        string nombreChat = label4.Text; // Ya lo tenemos en el label

                        reader.Close();

                        // Abrir el formulario de integrantes
                        FormVerIntegrantes formIntegrantes = new FormVerIntegrantes(
                            idChatActual,
                            nombreChat,
                            esIndividual
                        );
                        formIntegrantes.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar información: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarChats()
        {
            try
            {
                // Limpiar chats anteriores
                foreach (Control ctrl in panelChats.Controls.OfType<ChatItem>().ToList())
                {
                    ctrl.Dispose();
                }

                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = @"
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

                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                            nombreChat = reader.IsDBNull(reader.GetOrdinal("nombre_otro_usuario"))
                                ? "Usuario"
                                : reader.GetString("nombre_otro_usuario");
                        }
                        else
                        {
                            nombreChat = reader.IsDBNull(reader.GetOrdinal("nombre_chat"))
                                ? "Grupo sin nombre"
                                : reader.GetString("nombre_chat");
                        }

                        // Crear ChatItem
                        ChatItem chatItem = new ChatItem(idChat, idUsuarioActual, nombreChat, esIndividual);
                        chatItem.Location = new Point(panel4.Width + 5, yPosition);
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
            idChatActual = chatItem.IdChat;

            // Actualizar UI
            label4.Text = chatItem.NombreChat;
            panel7.Visible = false;
            richTextBox1.Visible = true;
            richTextBox2.ReadOnly = true;

            // Cargar mensajes
            CargarMensajes(idChatActual);

            // Resaltar el chat seleccionado
            foreach (Control ctrl in panelChats.Controls.OfType<ChatItem>())
            {
                ChatItem item = (ChatItem)ctrl;
                item.BackColor = (item.IdChat == idChatActual) ? Color.LightBlue : Color.White;
            }
        }

        private void CargarMensajes(int idChat)
        {
            try
            {
                richTextBox2.Clear();

                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT m.mensaje, m.fecha, u.nombre
                        FROM mensajes m
                        JOIN usuarios u ON m.id_usuario = u.id_usuario
                        WHERE m.id_chat = @idChat
                        ORDER BY m.fecha ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idChat", idChat);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            int contadorMensajes = 0;

                            while (reader.Read())
                            {
                                contadorMensajes++;
                                string nombre = reader.GetString("nombre");
                                string mensaje = reader.GetString("mensaje");
                                DateTime fecha = reader.GetDateTime("fecha");

                                // Agregar encabezado del mensaje
                                richTextBox2.SelectionColor = Color.DarkBlue;
                                richTextBox2.SelectionFont = new Font(richTextBox2.Font, FontStyle.Bold);
                                richTextBox2.AppendText($"[{fecha:HH:mm}] {nombre}: ");

                                // Resetear formato
                                richTextBox2.SelectionColor = Color.Black;
                                richTextBox2.SelectionFont = new Font(richTextBox2.Font, FontStyle.Regular);

                                // Verificar si es RTF
                                if (mensaje.Trim().StartsWith("{\\rtf"))
                                {
                                    try
                                    {
                                        IDataObject oldClipboardData = Clipboard.GetDataObject();
                                        Clipboard.SetText(mensaje, TextDataFormat.Rtf);

                                        int startPos = richTextBox2.SelectionStart;
                                        richTextBox2.Paste();

                                        if (oldClipboardData != null)
                                        {
                                            Clipboard.SetDataObject(oldClipboardData);
                                        }
                                    }
                                    catch
                                    {
                                        // Si falla el RTF, mostrar como texto plano
                                        richTextBox2.AppendText(mensaje);
                                    }
                                }
                                else
                                {
                                }

                                richTextBox2.AppendText(Environment.NewLine);
                            }

                            ultimoNumeroMensajes = contadorMensajes;
                        }
                    }
                }

                // Scroll automático al final
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mensajes: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimerActualizar_Tick(object sender, EventArgs e)
        {
            if (idChatActual == -1) return;

            try
            {
                // Verificar si hay nuevos mensajes
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM mensajes WHERE id_chat = @idChat";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idChat", idChatActual);

                    int numMensajes = Convert.ToInt32(cmd.ExecuteScalar());

                    // Si hay nuevos mensajes, recargar
                    if (numMensajes != ultimoNumeroMensajes)
                    {
                        int posicionAnterior = richTextBox2.SelectionStart;
                        CargarMensajes(idChatActual);

                        // Si estaba al final, mantener al final
                        if (posicionAnterior >= richTextBox2.Text.Length - 100)
                        {
                            richTextBox2.SelectionStart = richTextBox2.Text.Length;
                            richTextBox2.ScrollToCaret();
                        }
                    }
                }
            }
            catch
            {
                // Ignorar errores del timer
            }
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            // Validar que hay un chat seleccionado
            if (idChatActual == -1)
            {
                MessageBox.Show("Por favor selecciona un chat primero", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Validar mensaje no vacío
            string textoPlano = richTextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(textoPlano))
            {
                MessageBox.Show("No puedes enviar un mensaje vacío", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el mensaje (RTF si tiene imágenes, texto plano si no)
            string mensajeAGuardar;

            // Verificar si hay imágenes (emojis) en el mensaje
            if (richTextBox1.Rtf.Contains("\\pict"))
            {
                // Tiene imágenes, guardar como RTF
                mensajeAGuardar = richTextBox1.Rtf;
            }
            else
            {
                // Solo texto, guardar el texto plano
                mensajeAGuardar = textoPlano;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO mensajes (id_chat, id_usuario, mensaje, fecha)
                        VALUES (@idChat, @idUsuario, @mensaje, NOW())";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idChat", idChatActual);
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuarioActual);
                        cmd.Parameters.AddWithValue("@mensaje", mensajeAGuardar);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Recargar mensajes
                CargarMensajes(idChatActual);

                // Limpiar el campo de entrada
                richTextBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar mensaje: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RichTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Reemplazar emojis solo al presionar espacio o Enter
            if (e.KeyChar == ' ' || e.KeyChar == (char)Keys.Enter)
            {
                if (!reemplazando)
                {
                    reemplazando = true;
                    reemplazando = false;
                }
            }

            // Si presiona Enter, enviar el mensaje
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Evitar salto de línea
                PictureBox2_Click(sender, e);
            }
        }

        /*
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Ya no reemplaza emojis aquí para evitar lag
            // Los emojis se reemplazan en KeyPress
        }
        */
        
        /*
        private void InsertarImagenEnRichTextBox(Image imagen)
        {
            if (imagen == null) return;

            IDataObject oldClipboardData = Clipboard.GetDataObject();
            Clipboard.SetImage(imagen);
            richTextBox1.Paste();
            if (oldClipboardData != null)
            {
                Clipboard.SetDataObject(oldClipboardData);
            }
        }

        private void InsertarImagenEnChat(RichTextBox destino, Image imagen)
        {
            if (imagen == null) return;

            IDataObject oldClipboardData = Clipboard.GetDataObject();
            Clipboard.SetImage(imagen);
            destino.Paste();
            if (oldClipboardData != null)
            {
                Clipboard.SetDataObject(oldClipboardData);
            }
        }
        */
        

        private void button1_Click(object sender, EventArgs e)
        {
            // Mostrar opciones: Agregar Usuario o Crear Grupo
            

            
                // Agregar usuario individual
                FormAgregarUsuario formAgregar = new FormAgregarUsuario(idUsuarioActual);

               
                    int idUsuarioSeleccionado = formAgregar.IdUsuarioSeleccionado;
                    string nombreUsuarioSeleccionado = formAgregar.NombreUsuarioSeleccionado;

                    int? idChatExistente = VerificarChatIndividualExistente(idUsuarioSeleccionado);

                    if (idChatExistente.HasValue)
                    {
                        MessageBox.Show($"Ya tienes un chat con {nombreUsuarioSeleccionado}",
                            "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarChats();
                    }
                    else
                    {
                        CrearChatIndividual(idUsuarioSeleccionado, nombreUsuarioSeleccionado);
                    }
        }

        private void logOut_Button_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro de que quieres cerrar sesión?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                timerActualizar.Stop();
                this.Close();

                foreach (Form form in Application.OpenForms)
                {
                    if (form is Form1)
                    {
                        form.Show();
                        return;
                    }
                }

                Form1 loginForm = new Form1();
                loginForm.Show();
            }
        }

        private void CrearChatIndividual(int idOtroUsuario, string nombreOtroUsuario)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string queryChat = "INSERT INTO chats (nombre_chat, es_individual) VALUES (NULL, 1)";
                        MySqlCommand cmdChat = new MySqlCommand(queryChat, conn, transaction);
                        cmdChat.ExecuteNonQuery();

                        int idNuevoChat = (int)cmdChat.LastInsertedId;

                        string queryUsuariosChats = "INSERT INTO usuarios_chats (id_usuario, id_chat) VALUES (@idUsuario, @idChat)";

                        MySqlCommand cmdUsuario1 = new MySqlCommand(queryUsuariosChats, conn, transaction);
                        cmdUsuario1.Parameters.AddWithValue("@idUsuario", idUsuarioActual);
                        cmdUsuario1.Parameters.AddWithValue("@idChat", idNuevoChat);
                        cmdUsuario1.ExecuteNonQuery();

                        MySqlCommand cmdUsuario2 = new MySqlCommand(queryUsuariosChats, conn, transaction);
                        cmdUsuario2.Parameters.AddWithValue("@idUsuario", idOtroUsuario);
                        cmdUsuario2.Parameters.AddWithValue("@idChat", idNuevoChat);
                        cmdUsuario2.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show($"Chat con {nombreOtroUsuario} creado exitosamente",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void CrearGrupo(string nombreGrupo, List<int> miembros)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 1. Crear el grupo
                        string queryChat = "INSERT INTO chats (nombre_chat, es_individual) VALUES (@nombre, 0)";
                        MySqlCommand cmdChat = new MySqlCommand(queryChat, conn, transaction);
                        cmdChat.Parameters.AddWithValue("@nombre", nombreGrupo);
                        cmdChat.ExecuteNonQuery();

                        int idNuevoGrupo = (int)cmdChat.LastInsertedId;

                        // 2. Agregar al usuario actual
                        string queryUsuariosChats = "INSERT INTO usuarios_chats (id_usuario, id_chat) VALUES (@idUsuario, @idChat)";
                        MySqlCommand cmdUsuarioActual = new MySqlCommand(queryUsuariosChats, conn, transaction);
                        cmdUsuarioActual.Parameters.AddWithValue("@idUsuario", idUsuarioActual);
                        cmdUsuarioActual.Parameters.AddWithValue("@idChat", idNuevoGrupo);
                        cmdUsuarioActual.ExecuteNonQuery();

                        // 3. Agregar a los miembros seleccionados
                        foreach (int idMiembro in miembros)
                        {
                            MySqlCommand cmdMiembro = new MySqlCommand(queryUsuariosChats, conn, transaction);
                            cmdMiembro.Parameters.AddWithValue("@idUsuario", idMiembro);
                            cmdMiembro.Parameters.AddWithValue("@idChat", idNuevoGrupo);
                            cmdMiembro.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show($"Grupo '{nombreGrupo}' creado exitosamente con {miembros.Count} miembros",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show("Error al crear grupo: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? VerificarChatIndividualExistente(int idOtroUsuario)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

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

        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel6_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerActualizar?.Stop();
            base.OnFormClosing(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormCrearGrupo formGrupo = new FormCrearGrupo(idUsuarioActual);

            if (formGrupo.ShowDialog() == DialogResult.OK)
            {
                string nombreGrupo = formGrupo.NombreGrupo;
                List<int> miembros = formGrupo.UsuariosSeleccionados;

                CrearGrupo(nombreGrupo, miembros);
            }
        }
    }
}