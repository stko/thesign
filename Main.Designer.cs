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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.keyDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filesSignaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugtrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtoncheckEmailVality = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSendFileandSig = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSendSignaturesOnly = new System.Windows.Forms.ToolStripButton();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.AllowDrop = true;
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Output.Location = new System.Drawing.Point(0, 55);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Output.Size = new System.Drawing.Size(646, 226);
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
            this.toolStripMenuItem1,
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
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyDataToolStripMenuItem,
            this.filesSignaturesToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.toolStripMenuItem1.Text = "&Backup...";
            // 
            // keyDataToolStripMenuItem
            // 
            this.keyDataToolStripMenuItem.Name = "keyDataToolStripMenuItem";
            this.keyDataToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.keyDataToolStripMenuItem.Text = "&Key Data...";
            this.keyDataToolStripMenuItem.Click += new System.EventHandler(this.keyDataToolStripMenuItem_Click);
            // 
            // filesSignaturesToolStripMenuItem
            // 
            this.filesSignaturesToolStripMenuItem.Name = "filesSignaturesToolStripMenuItem";
            this.filesSignaturesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.filesSignaturesToolStripMenuItem.Text = "&Files && Signatures...";
            this.filesSignaturesToolStripMenuItem.Click += new System.EventHandler(this.filesSignaturesToolStripMenuItem_Click);
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
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.aboutToolStripMenuItem.Text = "&About..";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            // toolStripContainer1
            // 
            this.toolStripContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(646, 4);
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 27);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(646, 29);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtoncheckEmailVality,
            this.toolStripSeparator2,
            this.toolStripButtonSendFileandSig,
            this.toolStripButtonSendSignaturesOnly});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(87, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtoncheckEmailVality
            // 
            this.toolStripButtoncheckEmailVality.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtoncheckEmailVality.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtoncheckEmailVality.Image")));
            this.toolStripButtoncheckEmailVality.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtoncheckEmailVality.Name = "toolStripButtoncheckEmailVality";
            this.toolStripButtoncheckEmailVality.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtoncheckEmailVality.Text = "toolStripButton1";
            this.toolStripButtoncheckEmailVality.ToolTipText = "Check Validity of actual selected Email";
            this.toolStripButtoncheckEmailVality.Click += new System.EventHandler(this.toolStripButtoncheckEmailVality_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSendFileandSig
            // 
            this.toolStripButtonSendFileandSig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSendFileandSig.Enabled = false;
            this.toolStripButtonSendFileandSig.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSendFileandSig.Image")));
            this.toolStripButtonSendFileandSig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSendFileandSig.Name = "toolStripButtonSendFileandSig";
            this.toolStripButtonSendFileandSig.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSendFileandSig.Text = "toolStripButton1";
            this.toolStripButtonSendFileandSig.ToolTipText = "Send File and Signature...";
            this.toolStripButtonSendFileandSig.Click += new System.EventHandler(this.toolStripButtonSendFileandSig_Click);
            // 
            // toolStripButtonSendSignaturesOnly
            // 
            this.toolStripButtonSendSignaturesOnly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSendSignaturesOnly.Enabled = false;
            this.toolStripButtonSendSignaturesOnly.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSendSignaturesOnly.Image")));
            this.toolStripButtonSendSignaturesOnly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSendSignaturesOnly.Name = "toolStripButtonSendSignaturesOnly";
            this.toolStripButtonSendSignaturesOnly.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSendSignaturesOnly.Text = "toolStripButtonSendSignaturesOnly";
            this.toolStripButtonSendSignaturesOnly.ToolTipText = "Send Signatures Only";
            this.toolStripButtonSendSignaturesOnly.Click += new System.EventHandler(this.toolStripButtonSendSignaturesOnly_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 280);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "TheSign - (C) Steffen Köhler ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtoncheckEmailVality;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSendFileandSig;
        private System.Windows.Forms.ToolStripButton toolStripButtonSendSignaturesOnly;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem keyDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filesSignaturesToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

