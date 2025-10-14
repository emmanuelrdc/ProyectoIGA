namespace chat
{
    partial class FormVerIntegrantes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblNombreGrupo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.listBoxIntegrantes = new System.Windows.Forms.ListBox();
            this.lblContador = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(148, 20);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Información";
            // 
            // lblNombreGrupo
            // 
            this.lblNombreGrupo.AutoSize = true;
            this.lblNombreGrupo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblNombreGrupo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblNombreGrupo.Location = new System.Drawing.Point(15, 45);
            this.lblNombreGrupo.Name = "lblNombreGrupo";
            this.lblNombreGrupo.Size = new System.Drawing.Size(162, 24);
            this.lblNombreGrupo.TabIndex = 1;
            this.lblNombreGrupo.Text = "Nombre Grupo";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSubtitulo.Location = new System.Drawing.Point(15, 85);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(93, 17);
            this.lblSubtitulo.TabIndex = 2;
            this.lblSubtitulo.Text = "Integrantes:";
            // 
            // listBoxIntegrantes
            // 
            this.listBoxIntegrantes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.listBoxIntegrantes.FormattingEnabled = true;
            this.listBoxIntegrantes.ItemHeight = 16;
            this.listBoxIntegrantes.Location = new System.Drawing.Point(15, 110);
            this.listBoxIntegrantes.Name = "listBoxIntegrantes";
            this.listBoxIntegrantes.Size = new System.Drawing.Size(360, 260);
            this.listBoxIntegrantes.TabIndex = 3;
            // 
            // lblContador
            // 
            this.lblContador.AutoSize = true;
            this.lblContador.ForeColor = System.Drawing.Color.Gray;
            this.lblContador.Location = new System.Drawing.Point(15, 380);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(107, 13);
            this.lblContador.TabIndex = 4;
            this.lblContador.Text = "0 miembros en total";
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.LightGray;
            this.btnCerrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.Location = new System.Drawing.Point(140, 410);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(120, 35);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.lblTitulo);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 50);
            this.panel1.TabIndex = 6;
            // 
            // FormVerIntegrantes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 460);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.listBoxIntegrantes);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblNombreGrupo);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormVerIntegrantes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Integrantes del Grupo";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNombreGrupo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.ListBox listBoxIntegrantes;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Panel panel1;
    }
}