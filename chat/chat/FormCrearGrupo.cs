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
    public partial class FormCrearGrupo : Form
    {
        private int idUsuarioActual;
        private DataTable dtUsuarios;
        public string NombreGrupo { get; private set; }
        public List<int> UsuariosSeleccionados { get; private set; }

        public FormCrearGrupo(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
            UsuariosSeleccionados = new List<int>();
            CargarUsuarios();
        }
        private void CargarUsuarios()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    // Cargar solo usuarios con los que ya tienes chat
                    string query = @"
                        SELECT DISTINCT u.id_usuario, u.nombre
                        FROM usuarios u
                        INNER JOIN usuarios_chats uc1 ON u.id_usuario = uc1.id_usuario
                        INNER JOIN usuarios_chats uc2 ON uc1.id_chat = uc2.id_chat
                        WHERE uc2.id_usuario = @idUsuario
                        AND u.id_usuario != @idUsuario
                        ORDER BY u.nombre";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuarioActual);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    dtUsuarios = new DataTable();
                    adapter.Fill(dtUsuarios);

                    foreach (DataRow row in dtUsuarios.Rows)
                    {
                        checkedListBoxUsuarios.Items.Add(new UsuarioItem
                        {
                            Id = Convert.ToInt32(row["id_usuario"]),
                            Nombre = row["nombre"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkedListBoxUsuarios_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Actualizar contador (se ejecuta después del cambio)
            this.BeginInvoke(new Action(() =>
            {
                int contador = checkedListBoxUsuarios.CheckedItems.Count;
                labelContador.Text = $"{contador} miembro{(contador != 1 ? "s" : "")} seleccionado{(contador != 1 ? "s" : "")}";
            }));
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            string nombreGrupo = txtNombreGrupo.Text.Trim();

            // Validar nombre del grupo
            if (string.IsNullOrEmpty(nombreGrupo))
            {
                MessageBox.Show("Por favor ingresa un nombre para el grupo", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreGrupo.Focus();
                return;
            }

            // Validar que haya al menos 2 miembros
            if (checkedListBoxUsuarios.CheckedItems.Count < 2)
            {
                MessageBox.Show("Debes seleccionar al menos 2 miembros para crear un grupo", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener IDs de usuarios seleccionados
            UsuariosSeleccionados.Clear();
            foreach (UsuarioItem item in checkedListBoxUsuarios.CheckedItems)
            {
                UsuariosSeleccionados.Add(item.Id);
            }

            NombreGrupo = nombreGrupo;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

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
