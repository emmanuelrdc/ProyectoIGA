using System.Net.Sockets;
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
        string IPserver = "192.168.0.101";
        int Port = 13000;

        public FormCrearGrupo(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario;
            UsuariosSeleccionados = new List<int>();
            this.Load += FormCrearGrupo_load;
            
        }
        private async void FormCrearGrupo_load(object sender, EventArgs e)
        {
            await CargarUsuarios();
        }

        private async Task<string> EnviarPeticion(string mensaje)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(IPserver, Port);

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

        private async Task CargarUsuarios()
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

                dtUsuarios = new DataTable();
                dtUsuarios.Columns.Add("id_usuario", typeof(int));
                dtUsuarios.Columns.Add("nombre", typeof(string));

                
                string[] users = partes[1].Split(';');  

                foreach (string u in users)
                {
                    if (string.IsNullOrEmpty(u)) continue;

                    
                    string[] datos = u.Split(',');  

                    if (datos.Length < 2)
                    {
                        Console.WriteLine($"Datos de usuario inválidos: {u}");
                        continue;
                    }

                    try
                    {
                        int idUsuario = int.Parse(datos[0]);
                        string nombre = datos[1];

                        DataRow row = dtUsuarios.NewRow();
                        row["id_usuario"] = idUsuario;
                        row["nombre"] = nombre;
                        dtUsuarios.Rows.Add(row);

                        checkedListBoxUsuarios.Items.Add(new UsuarioItem
                        {
                            Id = idUsuario,
                            Nombre = nombre
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error procesando usuario '{u}': {ex.Message}");
                    }
                }

                if (checkedListBoxUsuarios.Items.Count == 0)
                {
                    MessageBox.Show("No hay otros usuarios disponibles para agregar al grupo.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message + "\n" + ex.StackTrace,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkedListBoxUsuarios_ItemCheck(object sender, ItemCheckEventArgs e)
        {
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
