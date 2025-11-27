using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class Form1 : Form
    {
        string IPserver = "127.0.0.1";
        int PortServer = 13000;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string usuario = richTextBox1.Text.Trim();
            string password = richTextBox2.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || usuario == "textoejemplo@abc.com")
            {
                MessageBox.Show("Por favor ingresa tu usuario");
                return;
            }

            if (string.IsNullOrEmpty(password) || password == "contraseña")
            {
                MessageBox.Show("Por favor ingresa tu contraseña");
                return;
            }

            button1.Enabled = false;
            button2.Enabled = false;

            try
            {
                string mensaje = "LOGIN|" + usuario + "|" + password;
                string respuesta = await EnviarMensaje(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success")
                {
                    int userId = int.Parse(partes[1]);
                    string userName = partes[2];

                    MessageBox.Show("Login exitoso!");

                    menuChat formMenu = new menuChat(userId, userName);
                    formMenu.FormClosed += (s, args) => this.Show();
                    formMenu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos");
                    richTextBox1.Clear();
                    richTextBox2.Clear();
                }
            }
            catch
            {
                MessageBox.Show("Error al conectar con el servidor");
            }
            finally
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string usuario = richTextBox1.Text.Trim();
            string password = richTextBox2.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || usuario == "textoejemplo@abc.com")
            {
                MessageBox.Show("Por favor ingresa un usuario");
                return;
            }

            if (string.IsNullOrEmpty(password) || password == "contraseña")
            {
                MessageBox.Show("Por favor ingresa una contraseña");
                return;
            }

            if (usuario.Length < 3)
            {
                MessageBox.Show("El usuario debe tener al menos 3 caracteres");
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres");
                return;
            }

            button1.Enabled = false;
            button2.Enabled = false;

            try
            {
                string mensaje = "REGISTER|" + usuario + "|" + password;
                string respuesta = await EnviarMensaje(mensaje);
                string[] partes = respuesta.Split('|');

                if (partes[0] == "Success")
                {
                    MessageBox.Show("Cuenta creada con éxito! Ya puedes iniciar sesión");
                    richTextBox1.Clear();
                    richTextBox2.Clear();
                    richTextBox1.Text = "textoejemplo@abc.com";
                    richTextBox1.ForeColor = Color.Gray;
                    richTextBox2.Text = "contraseña";
                    richTextBox2.ForeColor = Color.Gray;
                }
                else
                {
                    MessageBox.Show(partes.Length > 1 ? partes[1] : "Error al crear la cuenta");
                }
            }
            catch
            {
                MessageBox.Show("Error al conectar con el servidor");
            }
            finally
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private async Task<string> EnviarMensaje(string mensaje)
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(IPserver, PortServer);

                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(mensaje);
                    await stream.WriteAsync(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
            }
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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
