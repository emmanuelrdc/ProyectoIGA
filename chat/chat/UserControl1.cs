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
    public partial class ChatItem : UserControl
    {
        public int IdChat { get; set; }
        public int IdUsuario { get; set; }
        public string NombreChat { get; set; }
        public bool EsIndividual { get; set; }
        public event EventHandler ChatClicked;
        public ChatItem()
        {
            InitializeComponent();
            this.Cursor = Cursors.Hand;
            this.Click += ChatItem_Click;
        }
        public ChatItem(int idChat, int idUsuario, string nombreChat, bool esIndividual) : this()
        {
            IdChat = idChat;
            IdUsuario = idUsuario;
            NombreChat = nombreChat;
            EsIndividual = esIndividual;

            lblNombreChat.Text = nombreChat;
        }
        private void ChatItem_Click(object sender, EventArgs e)
        {
            ChatClicked?.Invoke(this, e);
        }
        private void ChatItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(230, 230, 230);
        }

        private void ChatItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        public void SetUltimoMensaje(string mensaje)
        {
            lblUltimoMensaje.Text = mensaje.Length > 30 ? mensaje.Substring(0, 30) + "..." : mensaje;
        }
    }
}
