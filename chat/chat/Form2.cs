using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        string IPserver = "127.0.0.1";
        int PortServer = 13000;

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
            this.idUsuarioActual = idUsuario;
            this.nombreUsuarioActual = nombreUsuario;
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

        private async Task<string> EnviarPeticion(string mensaje)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(IPserver, PortServer);

                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(mensaje);
                        await stream.WriteAsync(data, 0, data.Length);

                        byte[] buffer = new byte[4096];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error|No se pudo conectar al servidor" + ex;
                ;
            }
        }

        private async void CargarChats()
        {
            try
            {
                string mensaje = "GETCHATS|" + idUsuarioActual;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] != "Success" || partes.Length < 2)
                {
                    MessageBox.Show("Error al cargar chats");
                    return;
                }

                string[] chats = partes[1].Split(',');
                int ypos = 0;

                foreach (string chat in chats)
                {
                    if (string.IsNullOrEmpty(chat)) continue;

                    string[] datosChat = chat.Split(';');
                    if (datosChat.Length < 3) continue;

                    int idChat = int.Parse(datosChat[0]);
                    string nomChat = datosChat[1];
                    bool esIndividual = datosChat[2] == "1";

                    ChatItem chatItem = new ChatItem(idChat, idUsuarioActual, nomChat, esIndividual);
                    chatItem.Location = new Point(panel4.Width + 5, ypos);
                    chatItem.Width = panelChats.Width - panel4.Width - 25;
                    chatItem.ChatClicked += ChatItem_ChatClicked;

                    panelChats.Controls.Add(chatItem);
                    ypos += chatItem.Height + 5;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar chats: " + ex.Message);
            }
        }

        private async void CargarMensajes(int idChat)
        {
            try
            {
                richTextBox2.Clear();

                string mensage = "GETMESSAGES|" + idChat;
                string respuesta = await EnviarPeticion(mensage);
                string[] partes = respuesta.Split('|');

                if (partes[0] != "Success" || partes.Length < 2)
                {
                    return;
                }

                string[] mensajes = partes[1].Split(',');
                int contadorMensajes = mensajes.Length;

                foreach(string msg in mensajes)
                {
                    if (string.IsNullOrEmpty(msg)) continue;

                    string[] datosMensaje = msg.Split(',');
                    if (datosMensaje.Length < 3) continue;

                    string nombre = datosMensaje[0];
                    string textoMensaje = datosMensaje[1].Replace("<<COMA>>", ",");
                    string fecha = datosMensaje[2];

                    richTextBox2.SelectionColor = Color.DarkBlue;
                    richTextBox2.SelectionFont = new Font(richTextBox2.Font, FontStyle.Bold);
                    richTextBox2.AppendText($"[{fecha}] {nombre}: ");

                    richTextBox2.SelectionColor = Color.Black;
                    richTextBox2.SelectionFont = new Font(richTextBox2.Font, FontStyle.Regular);

                    if (textoMensaje.Trim().StartsWith("{\\rtf"))
                    {
                        try
                        {
                            IDataObject oldClipboardData = Clipboard.GetDataObject();
                            Clipboard.SetText(textoMensaje, TextDataFormat.Rtf);
                            richTextBox2.Paste();
                            if (oldClipboardData != null)
                            {
                                Clipboard.SetDataObject(oldClipboardData);
                            }
                        }
                        catch
                        {
                            richTextBox2.AppendText(textoMensaje);
                        }
                    }
                    else
                    {
                        richTextBox2.AppendText(textoMensaje);
                    }

                    richTextBox2.AppendText(Environment.NewLine);
                }

                ultimoNumeroMensajes = contadorMensajes;
                richTextBox2.SelectionStart = richTextBox2.Text.Length;
                richTextBox2.ScrollToCaret();
                
            }
            catch (Exception ex)
            {
            MessageBox.Show("Error al cargar mensajes: " + ex.Message);
            }
        }

        private async void PictureBox2_Click(object sender, EventArgs e)
        {
            if (idChatActual == -1)
            {
                MessageBox.Show("Por favor selecciona un chat primero");
                return;
            }

            string textoPlano = richTextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(textoPlano))
            {
                MessageBox.Show("No puedes enviar un mensaje vacío");
                return;
            }

            string mensajeAGuardar;

            if (richTextBox1.Rtf.Contains("\\pict"))
            {
                mensajeAGuardar = richTextBox1.Rtf;
            }
            else
            {
                mensajeAGuardar = textoPlano;
            }

            try
            {
                // Enviar mensaje al servidor
                // IMPORTANTE: Reemplazar comas por otro carácter para no romper el protocolo
                string mensajeLimpio = mensajeAGuardar.Replace(",", "<<COMA>>");
                string mensaje = "SENDMESSAGE|" + idChatActual + "|" + idUsuarioActual + "|" + mensajeLimpio;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success")
                {
                    // Recargar mensajes
                    CargarMensajes(idChatActual);

                    // Limpiar el campo de entrada
                    richTextBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Error al enviar mensaje");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar mensaje: " + ex.Message);
            }
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

        private async void TimerActualizar_Tick(object sender, EventArgs e)
        {
            if (idChatActual == -1) return;

            try
            {
                // Verificar si hay nuevos mensajes
                string mensaje = "COUNTMESSAGES|" + idChatActual;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success" && partes.Length >= 2)
                {
                    int numMensajes = int.Parse(partes[1]);

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

        private async void button1_Click(object sender, EventArgs e)
        {
            FormAgregarUsuario formAgregar = new FormAgregarUsuario(idUsuarioActual);

            if (formAgregar.ShowDialog() == DialogResult.OK)
            {
                int idUsuarioSeleccionado = formAgregar.IdUsuarioSeleccionado;
                string nombreUsuarioSeleccionado = formAgregar.NombreUsuarioSeleccionado;

                // Verificar si ya existe el chat
                string mensaje = "VERIFYCHAT|" + idUsuarioActual + "|" + idUsuarioSeleccionado;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success")
                {
                    MessageBox.Show($"Ya tienes un chat con {nombreUsuarioSeleccionado}");
                    CargarChats();
                }
                else
                {
                    // Crear nuevo chat
                    mensaje = "CREATECHAT|" + idUsuarioActual + "|" + idUsuarioSeleccionado;
                    respuesta = await EnviarPeticion(mensaje);
                    partes = respuesta.Split('|');

                    if (partes[0] == "Success")
                    {
                        MessageBox.Show($"Chat con {nombreUsuarioSeleccionado} creado exitosamente");
                        CargarChats();
                    }
                    else
                    {
                        MessageBox.Show("Error al crear chat");
                    }
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FormCrearGrupo formGrupo = new FormCrearGrupo(idUsuarioActual);

            if (formGrupo.ShowDialog() == DialogResult.OK)
            {
                string nombreGrupo = formGrupo.NombreGrupo;
                List<int> miembros = formGrupo.UsuariosSeleccionados;

                // Convertir lista a string
                string miembrosStr = string.Join(",", miembros);

                string mensaje = "CREATEGROUP|" + idUsuarioActual + "|" + nombreGrupo + "|" + miembrosStr;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success")
                {
                    MessageBox.Show($"Grupo '{nombreGrupo}' creado exitosamente con {miembros.Count} miembros");
                    CargarChats();
                }
                else
                {
                    MessageBox.Show("Error al crear grupo");
                }
            }
        }

        private async void Label4_Click(object sender, EventArgs e)
        {
            if (idChatActual == -1)
            {
                MessageBox.Show("Por favor selecciona un chat primero");
                return;
            }

            string mensaje = "GETCHATMEMBERS|" + idChatActual;
            string respuesta = await EnviarPeticion(mensaje);
            string[] partes = respuesta.Split('|');

            if (partes[0] == "Success" && partes.Length >= 2)
            {
                // Abrir formulario de integrantes
                FormVerIntegrantes formIntegrantes = new FormVerIntegrantes(
                    idChatActual,
                    label4.Text,
                    true/*partes[1]*/ // String con los miembros
                );
                formIntegrantes.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error al cargar integrantes");
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
        /*
        //cambiar
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
        }*/
        /*
        //cambiar
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
        */
        /*
       //cambiar
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
        */
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel6_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerActualizar?.Stop();
            base.OnFormClosing(e);
        }
        
    }
}