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
        string IPserver = "10.103.84.191";
        int PortServer = 13000;
        private bool pantallaCompleta = false;
        private FormWindowState estadoAnterior;
        private Rectangle limiteAnterior;
        public Form1()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }
        private void ConfigurarFormulario()
        {
            // Configurar el formulario para que sea redimensionable
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(600, 400);

            // Suscribir al evento de redimensionamiento
            this.Resize += Form1_Resize;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // Configurar anclajes iniciales
            ConfigurarAnclajes();
        }
        private void ConfigurarAnclajes()
        {
            // El groupBox1 ya tiene Dock = Fill, lo cual está bien
            // Ahora configuramos los controles internos para que se ajusten

            // Los RichTextBox y TextBox se anclarán al centro
            AjustarPosicionCentrada();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Ajustar la posición de los controles cuando cambie el tamaño
            AjustarPosicionCentrada();
        }
        private void AjustarPosicionCentrada()
        {
            // Obtener el centro del formulario
            int centroX = this.ClientSize.Width / 2;
            int centroY = this.ClientSize.Height / 2;

            // Ajustar el texto de bienvenida
            textBox1.Location = new Point(centroX - textBox1.Width / 2, centroY - 120);

            // Ajustar richTextBox1 (usuario)
            int anchoRTB1 = Math.Min(295, this.ClientSize.Width - 100);
            richTextBox1.Width = anchoRTB1;
            richTextBox1.Location = new Point(centroX - richTextBox1.Width / 2, centroY - 50);

            // Ajustar richTextBox2 (contraseña)
            int anchoRTB2 = Math.Min(246, this.ClientSize.Width - 100);
            richTextBox2.Width = anchoRTB2;
            richTextBox2.Location = new Point(centroX - richTextBox2.Width / 2, centroY - 10);

            // Ajustar botón de iniciar sesión
            button1.Location = new Point(centroX - button1.Width / 2, centroY + 40);

            // Ajustar botón de crear cuenta
            button2.Location = new Point(centroX - button2.Width / 2, centroY + 80);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // F11 para pantalla completa
            if (e.KeyCode == Keys.F11)
            {
                AlternarPantallaCompleta();
            }
            // ESC para salir de pantalla completa
            else if (e.KeyCode == Keys.Escape && pantallaCompleta)
            {
                SalirPantallaCompleta();
            }
        }

        private void AlternarPantallaCompleta()
        {
            if (!pantallaCompleta)
            {
                EntrarPantallaCompleta();
            }
            else
            {
                SalirPantallaCompleta();
            }
        }

        private void EntrarPantallaCompleta()
        {
            // Guardar el estado actual
            estadoAnterior = this.WindowState;
            limiteAnterior = this.Bounds;

            // Configurar pantalla completa
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            pantallaCompleta = true;
        }

        private void SalirPantallaCompleta()
        {
            // Restaurar el estado anterior
            this.TopMost = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = estadoAnterior;

            if (estadoAnterior == FormWindowState.Normal)
            {
                this.Bounds = limiteAnterior;
            }

            pantallaCompleta = false;
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
            AjustarPosicionCentrada();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
