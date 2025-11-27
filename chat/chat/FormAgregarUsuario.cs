using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;

namespace chat
{
    public partial class FormAgregarUsuario : Form
    {
        private int idUsuarioActual;
        private DataTable Usuarios;
        public int IdUsuarioSeleccionado { get; private set; }
        public string NombreUsuarioSeleccionado { get; private set; }
        string IPserver = "127.0.0.1";
        int PortServer = 13000;
        public FormAgregarUsuario(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
            this.Load += FormAgregarUsuario_Load;
        }
        private void FormAgregarUsuario_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
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
                return "Error|No se pudo conectar al servidor";
            }
        }

        private async void CargarUsuarios()
        {
            try
            {
                string mensaje = "GETALLUSERS|" + idUsuarioActual;
                string respuesta = await EnviarPeticion(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] != "Success" || partes.Length < 2)
                {
                    MessageBox.Show("Error al cargar usuarios: " + (partes.Length > 1 ? partes[1] : "Sin respuesta"));
                    return;
                }

                Usuarios = new DataTable();
                Usuarios.Columns.Add("id_usuario", typeof(int));
                Usuarios.Columns.Add("nombre", typeof(string));

                string[] user = partes[1].Split(';');
                foreach (string u in user)
                {
                    if (string.IsNullOrEmpty(u)) continue;

                    string[] datos = u.Split(',');
                    if (datos.Length < 2) continue;

                    try
                    {
                        DataRow row = Usuarios.NewRow();
                        row["id_usuario"] = int.Parse(datos[0]);
                        row["nombre"] = datos[1];
                        Usuarios.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error procesando usuario '{u}': {ex.Message}");
                    }

                }
                ActualizarListBox();
                if (Usuarios.Rows.Count == 0)
                {
                    MessageBox.Show("No hay otros usuarios registrados en el sistema.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los usuarios: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void ActualizarListBox()
        {
            listBoxUsuarios.Items.Clear();
            foreach (DataRow row in Usuarios.Rows)
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
            foreach (DataRow row in  Usuarios.Rows)
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

        private void listBoxUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}