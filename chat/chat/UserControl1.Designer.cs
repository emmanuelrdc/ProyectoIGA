namespace chat
{
    partial class ChatItem
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblNombreChat = new System.Windows.Forms.Label();
            this.pictureBoxFoto = new System.Windows.Forms.PictureBox();
            this.lblUltimoMensaje = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFoto)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNombreChat
            // 
            this.lblNombreChat.AutoSize = true;
            this.lblNombreChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblNombreChat.Location = new System.Drawing.Point(90, 10);
            this.lblNombreChat.Name = "lblNombreChat";
            this.lblNombreChat.Size = new System.Drawing.Size(120, 17);
            this.lblNombreChat.TabIndex = 0;
            this.lblNombreChat.Text = "Nombre Chat";
            this.lblNombreChat.Click += new System.EventHandler(this.ChatItem_Click);
            // 
            // pictureBoxFoto
            // 
            this.pictureBoxFoto.BackColor = System.Drawing.Color.LightGray;
            this.pictureBoxFoto.Location = new System.Drawing.Point(10, 10);
            this.pictureBoxFoto.Name = "pictureBoxFoto";
            this.pictureBoxFoto.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxFoto.TabIndex = 1;
            this.pictureBoxFoto.TabStop = false;
            this.pictureBoxFoto.Click += new System.EventHandler(this.ChatItem_Click);
            // 
            // lblUltimoMensaje
            // 
            this.lblUltimoMensaje.AutoSize = true;
            this.lblUltimoMensaje.ForeColor = System.Drawing.Color.Gray;
            this.lblUltimoMensaje.Location = new System.Drawing.Point(90, 35);
            this.lblUltimoMensaje.Name = "lblUltimoMensaje";
            this.lblUltimoMensaje.Size = new System.Drawing.Size(150, 13);
            this.lblUltimoMensaje.TabIndex = 2;
            this.lblUltimoMensaje.Text = "Último mensaje...";
            this.lblUltimoMensaje.Click += new System.EventHandler(this.ChatItem_Click);
            // 
            // ChatItem
            // 
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblUltimoMensaje);
            this.Controls.Add(this.pictureBoxFoto);
            this.Controls.Add(this.lblNombreChat);
            this.Name = "ChatItem";
            this.Size = new System.Drawing.Size(300, 90);
            this.MouseEnter += new System.EventHandler(this.ChatItem_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ChatItem_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblNombreChat;
        private System.Windows.Forms.PictureBox pictureBoxFoto;
        private System.Windows.Forms.Label lblUltimoMensaje;
        #endregion
    }
}
