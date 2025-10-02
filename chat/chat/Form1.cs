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
        Form2 f= null;
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
                                    f = new Form2();
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
    }
}
