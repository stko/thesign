namespace TheSign
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Output = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.theSignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printCertificateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugtrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ActionButton = new System.Windows.Forms.Button();
            this.ActionComboBox = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.AllowDrop = true;
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Output.Location = new System.Drawing.Point(0, 27);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Output.Size = new System.Drawing.Size(646, 220);
            this.Output.TabIndex = 0;
            this.Output.Text = "Please drag the files to sign into here";
            this.Output.DragDrop += new System.Windows.Forms.DragEventHandler(this.Output_DragDrop);
            this.Output.DragEnter += new System.Windows.Forms.DragEventHandler(this.Output_DragEnter);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.theSignToolStripMenuItem,
            this.keyToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(646, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // theSignToolStripMenuItem
            // 
            this.theSignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printCertificateToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.theSignToolStripMenuItem.Name = "theSignToolStripMenuItem";
            this.theSignToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.theSignToolStripMenuItem.Text = "&TheSign";
            // 
            // printCertificateToolStripMenuItem
            // 
            this.printCertificateToolStripMenuItem.Name = "printCertificateToolStripMenuItem";
            this.printCertificateToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.printCertificateToolStripMenuItem.Text = "&View Certificate...";
            this.printCertificateToolStripMenuItem.Click += new System.EventHandler(this.Menu_printCertificate);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.Menu_Quit);
            // 
            // keyToolStripMenuItem
            // 
            this.keyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listKeysToolStripMenuItem});
            this.keyToolStripMenuItem.Name = "keyToolStripMenuItem";
            this.keyToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.keyToolStripMenuItem.Text = "&Keys";
            // 
            // listKeysToolStripMenuItem
            // 
            this.listKeysToolStripMenuItem.Name = "listKeysToolStripMenuItem";
            this.listKeysToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.listKeysToolStripMenuItem.Text = "&List Keys...";
            this.listKeysToolStripMenuItem.Click += new System.EventHandler(this.Menu_listKeys);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onlineManualToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.bugtrackerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // onlineManualToolStripMenuItem
            // 
            this.onlineManualToolStripMenuItem.Name = "onlineManualToolStripMenuItem";
            this.onlineManualToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.onlineManualToolStripMenuItem.Text = "Online &Manual...";
            this.onlineManualToolStripMenuItem.Click += new System.EventHandler(this.onlineManualToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.aboutToolStripMenuItem.Text = "&About..";
            // 
            // bugtrackerToolStripMenuItem
            // 
            this.bugtrackerToolStripMenuItem.Name = "bugtrackerToolStripMenuItem";
            this.bugtrackerToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.bugtrackerToolStripMenuItem.Text = "&Bugtracker...";
            this.bugtrackerToolStripMenuItem.Click += new System.EventHandler(this.bugtrackerToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ActionButton);
            this.panel1.Controls.Add(this.ActionComboBox);
            this.panel1.Location = new System.Drawing.Point(0, 248);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(646, 29);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Action:";
            // 
            // ActionButton
            // 
            this.ActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionButton.Location = new System.Drawing.Point(569, 3);
            this.ActionButton.Name = "ActionButton";
            this.ActionButton.Size = new System.Drawing.Size(65, 23);
            this.ActionButton.TabIndex = 3;
            this.ActionButton.Text = "Go...";
            this.ActionButton.UseVisualStyleBackColor = true;
            this.ActionButton.Click += new System.EventHandler(this.ActionButton_Click);
            // 
            // ActionComboBox
            // 
            this.ActionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionComboBox.FormattingEnabled = true;
            this.ActionComboBox.Location = new System.Drawing.Point(50, 5);
            this.ActionComboBox.Name = "ActionComboBox";
            this.ActionComboBox.Size = new System.Drawing.Size(513, 21);
            this.ActionComboBox.TabIndex = 2;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 280);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "TheSign - (C) Steffen Köhler ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Output;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem theSignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlineManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printCertificateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem bugtrackerToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ActionButton;
        private System.Windows.Forms.ComboBox ActionComboBox;
    }
}

