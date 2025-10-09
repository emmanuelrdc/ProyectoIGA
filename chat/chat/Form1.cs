using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace chat
{

    public partial class Form1 : Form
    {
        menuChat f = null;
        public Form1()
        {
            InitializeComponent();

            DatabaseConnection.Initialize(server: "127.0.0.1",
                database: "chat_app",
                user: "root",
                password: "root");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = richTextBox1.Text.Trim();
            string password = richTextBox2.Text.Trim();

            // Validar que no estén vacíos o con placeholder
            if (string.IsNullOrEmpty(usuario) || usuario == "textoejemplo@abc.com")
            {
                MessageBox.Show("Por favor ingresa tu usuario", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password) || password == "contraseña")
            {
                MessageBox.Show("Por favor ingresa tu contraseña", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox2.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                { 
                    conn.Open();
                    string query = "SELECT id_usuario, nombre FROM usuarios WHERE nombre = @usuario AND contraseña = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@password", password);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Login exitoso
                        int idUsuario = reader.GetInt32("id_usuario");
                        string nombreUsuario = reader.GetString("nombre");

                        reader.Close();
                        conn.Close();

                        MessageBox.Show($"¡Bienvenido {nombreUsuario}!", "Login exitoso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Abrir el menuChat
                        menuChat formMenu = new menuChat(idUsuario, nombreUsuario);
                        formMenu.FormClosed += (s, args) => this.Show(); // Mostrar login al cerrar menuChat
                        formMenu.Show();
                        this.Hide(); // Ocultar el login
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos", "Error de autenticación",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Limpiar campos
                        richTextBox2.Clear();
                        richTextBox1.Clear();
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error de conexión a la base de datos:\n{ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string usuario = richTextBox1.Text.Trim();
            string password = richTextBox2.Text.Trim();

            // Validar que no estén vacíos o con placeholder
            if (string.IsNullOrEmpty(usuario) || usuario == "Usuario")
            {
                MessageBox.Show("Por favor ingresa un nombre de usuario", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password) || password == "Contraseña")
            {
                MessageBox.Show("Por favor ingresa una contraseña", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox2.Focus();
                return;
            }

            // Validar longitud mínima
            if (usuario.Length < 3)
            {
                MessageBox.Show("El usuario debe tener al menos 3 caracteres", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crear el usuario en la base de datos
            try
            {
                // USAR DatabaseConnection.ConnectionString
                using (MySqlConnection conn = new MySqlConnection(DatabaseConnection.ConnectionString))
                {
                    conn.Open();

                    // Verificar si el usuario ya existe
                    string queryVerificar = "SELECT COUNT(*) FROM usuarios WHERE nombre = @usuario";
                    MySqlCommand cmdVerificar = new MySqlCommand(queryVerificar, conn);
                    cmdVerificar.Parameters.AddWithValue("@usuario", usuario);

                    int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                    if (existe > 0)
                    {
                        MessageBox.Show("Este nombre de usuario ya existe. Por favor elige otro.",
                            "Usuario existente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        richTextBox1.SelectAll();
                        richTextBox1.Focus();
                        return;
                    }

                    // Insertar el nuevo usuario
                    string queryInsertar = "INSERT INTO usuarios (nombre, contraseña) VALUES (@usuario, @password)";
                    MySqlCommand cmdInsertar = new MySqlCommand(queryInsertar, conn);
                    cmdInsertar.Parameters.AddWithValue("@usuario", usuario);
                    cmdInsertar.Parameters.AddWithValue("@password", password);

                    cmdInsertar.ExecuteNonQuery();

                    MessageBox.Show($"¡Cuenta creada exitosamente!\nUsuario: {usuario}\n\nYa puedes iniciar sesión.",
                        "Cuenta creada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar los campos
                    richTextBox1.Clear();
                    richTextBox2.Clear();
                    richTextBox1.Text = "Usuario";
                    richTextBox1.ForeColor = System.Drawing.Color.Gray;
                    richTextBox2.Text = "Contraseña";
                    richTextBox2.ForeColor = System.Drawing.Color.Gray;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error al crear la cuenta:\n{ex.Message}",
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "textoejemplo@abc.com")
            {
                richTextBox1.Text = "";
                richTextBox1.ForeColor = Color.Black;
            }
        }
        private void richTextBox2_Enter(object sender, EventArgs e)
        {
            if (richTextBox2.Text == "contraseña")
            {
                richTextBox2.Text = "";
                richTextBox2.ForeColor = Color.Black;
            }
        }
        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                richTextBox1.Text = "textoejemplo@abc.com";
                richTextBox1.ForeColor = Color.Gray;
            }
        }

        private void richTextBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox2.Text))
            {
                richTextBox2.Text = "contraseña";
                richTextBox2.ForeColor = Color.Gray;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "textoejemplo@abc.com";
            richTextBox1.ForeColor = Color.Gray;

            richTextBox2.Text = "contraseña";
            richTextBox2.ForeColor = Color.Gray;
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
