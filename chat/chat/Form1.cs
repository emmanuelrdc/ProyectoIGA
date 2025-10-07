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
        menuChat f= null;
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand com;
            MySqlConnection conn;
            MySqlDataReader reader;

            try
            {
                conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=root1234;database=test");
                conn.Open();
                bool band = false;

                com = new MySqlCommand("SELECT nombre, cont FROM usuario ",conn);

                reader = com.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        if (richTextBox1.Text == reader["nombre"].ToString())
                        {
                            if (richTextBox2.Text == reader["cont"].ToString())
                            {
                                band = true;
                                if (f == null)
                                {
                                    f = new menuChat();
                                    f.Show();
                                }
                                f.Focus();
                                //this.Close();
                                

                            }
                        }
                    }
                    if (!band)
                    {
                        MessageBox.Show("No se encuentra el usario");
                    }
                }
                else
                {
                    MessageBox.Show("No se encuentra el usuario");
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MySqlCommand com;
            MySqlConnection conn;
            MySqlDataReader reader;

            try
            {
                conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=root1234;database=test");
                conn.Open();
                string nom = textBox1.Text;
                bool band=false;
                int i = 1;
                com = new MySqlCommand("SELECT nombre, cont FROM usuario ", conn);

                reader = com.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        if (richTextBox1.Text == reader["nombre"].ToString())
                        {
                            if (richTextBox2.Text == reader["cont"].ToString())
                            {
                                band = true;
                                MessageBox.Show("El usuario ya se encuentra registrado");
                            }
                        }
                        i++;
                    }
                }
                if (band == false || reader.HasRows == false)
                {
                    reader.Close();
                    com = new MySqlCommand("INSERT INTO usuario (idusuario, nombre, cont) VALUES (" + i + ", '" + richTextBox1.Text + "','" + richTextBox2.Text + "' ) ", conn);
                    i++;
                    int res = com.ExecuteNonQuery();
                    MessageBox.Show("usuario creado");
                }
                
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
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
            richTextBox2.Text = "contraseña";
            richTextBox2.ForeColor = Color.Gray;
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
