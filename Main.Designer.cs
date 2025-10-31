namespace fluxguard
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being being used.
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.projectUrlTxt = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backendToggle = new System.Windows.Forms.Button();
            this.embeddingsToggle = new System.Windows.Forms.Button();
            this.backendEstado = new System.Windows.Forms.Label();
            this.embeddingsEstado = new System.Windows.Forms.Label();
            this.checkTimer = new System.Windows.Forms.Timer(this.components);
            this.backendLogBox = new System.Windows.Forms.RichTextBox();
            this.embeddingsLogBox = new System.Windows.Forms.RichTextBox();
            this.startWithWindowsChk = new System.Windows.Forms.CheckBox();
            this.startMinimizedBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectUrlTxt
            // 
            this.projectUrlTxt.Location = new System.Drawing.Point(12, 12);
            this.projectUrlTxt.Name = "projectUrlTxt";
            this.projectUrlTxt.Size = new System.Drawing.Size(568, 20);
            this.projectUrlTxt.TabIndex = 0;
            this.projectUrlTxt.Text = "C:\\Users\\soperaciones\\Desktop\\pastoflux";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Fluxguard (Pastoflux)";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.salirToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(97, 26);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // backendToggle
            // 
            this.backendToggle.Location = new System.Drawing.Point(299, 46);
            this.backendToggle.Name = "backendToggle";
            this.backendToggle.Size = new System.Drawing.Size(75, 23);
            this.backendToggle.TabIndex = 3;
            this.backendToggle.Text = "Servidor";
            this.backendToggle.UseVisualStyleBackColor = true;
            this.backendToggle.Click += new System.EventHandler(this.backendToggle_Click);
            // 
            // embeddingsToggle
            // 
            this.embeddingsToggle.Location = new System.Drawing.Point(12, 46);
            this.embeddingsToggle.Name = "embeddingsToggle";
            this.embeddingsToggle.Size = new System.Drawing.Size(75, 23);
            this.embeddingsToggle.TabIndex = 7;
            this.embeddingsToggle.Text = "Embeddings";
            this.embeddingsToggle.UseVisualStyleBackColor = true;
            this.embeddingsToggle.Click += new System.EventHandler(this.embeddingsToggle_Click);
            // 
            // backendEstado
            // 
            this.backendEstado.AutoSize = true;
            this.backendEstado.Location = new System.Drawing.Point(380, 51);
            this.backendEstado.Name = "backendEstado";
            this.backendEstado.Size = new System.Drawing.Size(31, 13);
            this.backendEstado.TabIndex = 4;
            this.backendEstado.Text = "--------";
            // 
            // embeddingsEstado
            // 
            this.embeddingsEstado.AutoSize = true;
            this.embeddingsEstado.Location = new System.Drawing.Point(93, 51);
            this.embeddingsEstado.Name = "embeddingsEstado";
            this.embeddingsEstado.Size = new System.Drawing.Size(31, 13);
            this.embeddingsEstado.TabIndex = 8;
            this.embeddingsEstado.Text = "--------";
            // 
            // checkTimer
            // 
            this.checkTimer.Interval = 5000;
            this.checkTimer.Tick += new System.EventHandler(this.checkTimer_Tick);
            // 
            // backendLogBox
            // 
            this.backendLogBox.BackColor = System.Drawing.Color.Black;
            this.backendLogBox.ForeColor = System.Drawing.Color.LightGreen;
            this.backendLogBox.Location = new System.Drawing.Point(299, 75);
            this.backendLogBox.Name = "backendLogBox";
            this.backendLogBox.ReadOnly = true;
            this.backendLogBox.Size = new System.Drawing.Size(280, 194);
            this.backendLogBox.TabIndex = 9;
            this.backendLogBox.Text = "";
            // 
            // embeddingsLogBox
            // 
            this.embeddingsLogBox.BackColor = System.Drawing.Color.Black;
            this.embeddingsLogBox.ForeColor = System.Drawing.Color.LightGreen;
            this.embeddingsLogBox.Location = new System.Drawing.Point(12, 75);
            this.embeddingsLogBox.Name = "embeddingsLogBox";
            this.embeddingsLogBox.ReadOnly = true;
            this.embeddingsLogBox.Size = new System.Drawing.Size(280, 194);
            this.embeddingsLogBox.TabIndex = 10;
            this.embeddingsLogBox.Text = "";
            // 
            // startWithWindowsChk
            // 
            this.startWithWindowsChk.AutoSize = true;
            this.startWithWindowsChk.Location = new System.Drawing.Point(138, 279);
            this.startWithWindowsChk.Name = "startWithWindowsChk";
            this.startWithWindowsChk.Size = new System.Drawing.Size(122, 17);
            this.startWithWindowsChk.TabIndex = 11;
            this.startWithWindowsChk.Text = "Iniciar con Windows";
            this.startWithWindowsChk.UseVisualStyleBackColor = true;
            this.startWithWindowsChk.CheckedChanged += new System.EventHandler(this.startWithWindowsChk_CheckedChanged);
            // 
            // startMinimizedBtn
            // 
            this.startMinimizedBtn.Location = new System.Drawing.Point(12, 275);
            this.startMinimizedBtn.Name = "startMinimizedBtn";
            this.startMinimizedBtn.Size = new System.Drawing.Size(120, 23);
            this.startMinimizedBtn.TabIndex = 12;
            this.startMinimizedBtn.Text = "Iniciar minimizado";
            this.startMinimizedBtn.UseVisualStyleBackColor = true;
            this.startMinimizedBtn.Click += new System.EventHandler(this.startMinimizedBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 303);
            this.Controls.Add(this.projectUrlTxt);
            this.Controls.Add(this.startWithWindowsChk);
            this.Controls.Add(this.startMinimizedBtn);
            this.Controls.Add(this.backendToggle);
            this.Controls.Add(this.backendEstado);
            this.Controls.Add(this.embeddingsToggle);
            this.Controls.Add(this.embeddingsEstado);
            this.Controls.Add(this.backendLogBox);
            this.Controls.Add(this.embeddingsLogBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fluxguard - servicio para iniciar Pastoflux";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox projectUrlTxt;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.Button backendToggle;
        private System.Windows.Forms.Button embeddingsToggle;
        private System.Windows.Forms.Label backendEstado;
        private System.Windows.Forms.Label embeddingsEstado;
        private System.Windows.Forms.Timer checkTimer;
        private System.Windows.Forms.RichTextBox backendLogBox;
        private System.Windows.Forms.RichTextBox embeddingsLogBox;
        private System.Windows.Forms.CheckBox startWithWindowsChk;
        private System.Windows.Forms.Button startMinimizedBtn;
    }
}

