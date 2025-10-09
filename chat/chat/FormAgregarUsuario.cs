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
    public partial class FormAgregarUsuario : Form
    {
        private int idUsuarioActual;
        public int IdUsuarioSeleccionado { get; private set; }
        public string NombreUsuarioSeleccionado { get; private set; }
        public FormAgregarUsuario(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
        }
        private void CargarUsuarios()
        {
            try
            {
                // USAR DatabaseConnection.ConnectionString
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();
                    // Cargar todos los usuarios excepto el usuario actual
                    string query = "SELECT id_usuario, nombre FROM usuarios WHERE id_usuario != @idActual ORDER BY nombre";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idActual", idUsuarioActual);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    dtUsuarios = new DataTable();
                    adapter.Fill(dtUsuarios);

                    ActualizarListBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarListBox()
        {
            listBoxUsuarios.Items.Clear();
            foreach (DataRow row in dtUsuarios.Rows)
            {
                listBoxUsuarios.Items.Add(new UsuarioItem
                {
                    Id = Convert.ToInt32(row["id_usuario"]),
                    Nombre = row["nombre"].ToString()
                });
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                ActualizarListBox();
                return;
            }

            listBoxUsuarios.Items.Clear();
            foreach (DataRow row in dtUsuarios.Rows)
            {
                string nombre = row["nombre"].ToString().ToLower();
                if (nombre.Contains(filtro))
                {
                    listBoxUsuarios.Items.Add(new UsuarioItem
                    {
                        Id = Convert.ToInt32(row["id_usuario"]),
                        Nombre = row["nombre"].ToString()
                    });
                }
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (listBoxUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Por favor selecciona un usuario", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UsuarioItem usuarioSeleccionado = (UsuarioItem)listBoxUsuarios.SelectedItem;
            IdUsuarioSeleccionado = usuarioSeleccionado.Id;
            NombreUsuarioSeleccionado = usuarioSeleccionado.Nombre;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Clase auxiliar para mostrar usuarios en el ListBox
        private class UsuarioItem
        {
            public int Id { get; set; }
            public string Nombre { get; set; }

            public override string ToString()
            {
                return Nombre;
            }
        }
    }
}
