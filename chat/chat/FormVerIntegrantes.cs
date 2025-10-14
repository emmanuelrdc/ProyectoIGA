using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class FormVerIntegrantes : Form
    {
        private int idChat;
        private string nombreGrupo;
        private bool esIndividual;
        public FormVerIntegrantes(int chatId, string nombre, bool individual)
        {
            InitializeComponent();
            idChat = chatId;
            nombreGrupo = nombre;
            esIndividual = individual;

            this.Text = esIndividual ? "Información del Chat" : "Integrantes del Grupo";
            lblNombreGrupo.Text = nombreGrupo;

            CargarIntegrantes();
        }
        private void CargarIntegrantes()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT u.id_usuario, u.nombre
                        FROM usuarios u
                        INNER JOIN usuarios_chats uc ON u.id_usuario = uc.id_usuario
                        WHERE uc.id_chat = @idChat
                        ORDER BY u.nombre";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idChat", idChat);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    int contador = 0;
                    listBoxIntegrantes.Items.Clear();

                    while (reader.Read())
                    {
                        contador++;
                        string nombre = reader.GetString("nombre");

                        // Agregar icono según sea chat individual o grupo
                        string icono = esIndividual ? "👤" : "👥";
                        listBoxIntegrantes.Items.Add($"{icono} {nombre}");
                    }

                    reader.Close();

                    // Actualizar contador
                    if (esIndividual)
                    {
                        lblContador.Text = $"{contador} participante{(contador != 1 ? "s" : "")}";
                    }
                    else
                    {
                        lblContador.Text = $"{contador} miembro{(contador != 1 ? "s" : "")} en total";
                    }

                    // Si es chat individual, cambiar el subtítulo
                    if (esIndividual)
                    {
                        lblSubtitulo.Text = "Participantes:";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar integrantes: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
