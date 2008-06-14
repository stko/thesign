namespace TheSign
{
    partial class KeyForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyForm));
            this.keyview = new System.Windows.Forms.TreeView();
            this.TrustImagelist = new System.Windows.Forms.ImageList(this.components);
            this.KeyStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSign = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonTrust_dont_know = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrust_noTrust = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrustMarginal = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrustFully = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTrustultimate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.KeyStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // keyview
            // 
            this.keyview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keyview.HideSelection = false;
            this.keyview.ImageIndex = 0;
            this.keyview.ImageList = this.TrustImagelist;
            this.keyview.ItemHeight = 18;
            this.keyview.Location = new System.Drawing.Point(2, 3);
            this.keyview.Name = "keyview";
            this.keyview.SelectedImageIndex = 0;
            this.keyview.ShowNodeToolTips = true;
            this.keyview.Size = new System.Drawing.Size(592, 318);
            this.keyview.TabIndex = 5;
            this.keyview.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.keyview_AfterSelect);
            // 
            // TrustImagelist
            // 
            this.TrustImagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TrustImagelist.ImageStream")));
            this.TrustImagelist.TransparentColor = System.Drawing.Color.Transparent;
            this.TrustImagelist.Images.SetKeyName(0, "trust_dont_know.png");
            this.TrustImagelist.Images.SetKeyName(1, "trust_no_trust.png");
            this.TrustImagelist.Images.SetKeyName(2, "trust_marginally.png");
            this.TrustImagelist.Images.SetKeyName(3, "trust_fully.png");
            this.TrustImagelist.Images.SetKeyName(4, "trust_ultimatelly.png");
            this.TrustImagelist.Images.SetKeyName(5, "sign.png");
            // 
            // KeyStrip
            // 
            this.KeyStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.KeyStrip.Enabled = false;
            this.KeyStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripSign,
            this.toolStripSeparator2,
            this.toolStripButtonTrust_dont_know,
            this.toolStripButtonTrust_noTrust,
            this.toolStripButtonTrustMarginal,
            this.toolStripButtonTrustFully,
            this.toolStripButtonTrustultimate,
            this.toolStripSeparator4,
            this.toolStripButton2});
            this.KeyStrip.Location = new System.Drawing.Point(40, 0);
            this.KeyStrip.Name = "KeyStrip";
            this.KeyStrip.Size = new System.Drawing.Size(191, 25);
            this.KeyStrip.TabIndex = 6;
            this.KeyStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSign
            // 
            this.toolStripSign.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSign.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSign.Image")));
            this.toolStripSign.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSign.Name = "toolStripSign";
            this.toolStripSign.Size = new System.Drawing.Size(23, 22);
            this.toolStripSign.Text = "toolStripButton2";
            this.toolStripSign.ToolTipText = "Sign actual Key as Verified and Valid";
            this.toolStripSign.Click += new System.EventHandler(this.toolStripSign_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonTrust_dont_know
            // 
            this.toolStripButtonTrust_dont_know.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrust_dont_know.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTrust_dont_know.Image")));
            this.toolStripButtonTrust_dont_know.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrust_dont_know.Name = "toolStripButtonTrust_dont_know";
            this.toolStripButtonTrust_dont_know.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrust_dont_know.Tag = "1";
            this.toolStripButtonTrust_dont_know.Text = "toolStripButton3";
            this.toolStripButtonTrust_dont_know.ToolTipText = "Set TrustLevel: I don\'t know";
            this.toolStripButtonTrust_dont_know.Click += new System.EventHandler(this.toolStripButtonTrust_dont_know_Click);
            // 
            // toolStripButtonTrust_noTrust
            // 
            this.toolStripButtonTrust_noTrust.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrust_noTrust.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTrust_noTrust.Image")));
            this.toolStripButtonTrust_noTrust.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrust_noTrust.Name = "toolStripButtonTrust_noTrust";
            this.toolStripButtonTrust_noTrust.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrust_noTrust.Tag = "2";
            this.toolStripButtonTrust_noTrust.Text = "toolStripButton4";
            this.toolStripButtonTrust_noTrust.ToolTipText = "Set TrustLevel: I DONT trust";
            this.toolStripButtonTrust_noTrust.Click += new System.EventHandler(this.toolStripButtonTrust_dont_know_Click);
            // 
            // toolStripButtonTrustMarginal
            // 
            this.toolStripButtonTrustMarginal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrustMarginal.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTrustMarginal.Image")));
            this.toolStripButtonTrustMarginal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrustMarginal.Name = "toolStripButtonTrustMarginal";
            this.toolStripButtonTrustMarginal.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrustMarginal.Tag = "3";
            this.toolStripButtonTrustMarginal.Text = "toolStripButton5";
            this.toolStripButtonTrustMarginal.ToolTipText = "Set TrustLevel: I trust the key owner Marginally";
            this.toolStripButtonTrustMarginal.Click += new System.EventHandler(this.toolStripButtonTrust_dont_know_Click);
            // 
            // toolStripButtonTrustFully
            // 
            this.toolStripButtonTrustFully.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrustFully.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTrustFully.Image")));
            this.toolStripButtonTrustFully.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrustFully.Name = "toolStripButtonTrustFully";
            this.toolStripButtonTrustFully.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrustFully.Tag = "4";
            this.toolStripButtonTrustFully.Text = "toolStripButton6";
            this.toolStripButtonTrustFully.ToolTipText = "Set Trustlevel: I trust the key owner fully";
            this.toolStripButtonTrustFully.Click += new System.EventHandler(this.toolStripButtonTrust_dont_know_Click);
            // 
            // toolStripButtonTrustultimate
            // 
            this.toolStripButtonTrustultimate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTrustultimate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTrustultimate.Image")));
            this.toolStripButtonTrustultimate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTrustultimate.Name = "toolStripButtonTrustultimate";
            this.toolStripButtonTrustultimate.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTrustultimate.Tag = "5";
            this.toolStripButtonTrustultimate.Text = "toolStripButton7";
            this.toolStripButtonTrustultimate.ToolTipText = "Set Trustlevel: I trust the key owner Ultimatelly";
            this.toolStripButtonTrustultimate.Click += new System.EventHandler(this.toolStripButtonTrust_dont_know_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "Delete the key";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.keyview);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(597, 324);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(597, 349);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.KeyStrip);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(35, 25);
            this.toolStrip1.TabIndex = 7;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Send Key via Email...";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButtonSend_Click);
            // 
            // KeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 349);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KeyForm";
            this.Text = "TheSign - Public Key Ring";
            this.Shown += new System.EventHandler(this.KeyForm_Shown);
            this.KeyStrip.ResumeLayout(false);
            this.KeyStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView keyview;
        private System.Windows.Forms.ImageList TrustImagelist;
        private System.Windows.Forms.ToolStrip KeyStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripSign;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrust_dont_know;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrust_noTrust;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrustMarginal;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrustFully;
        private System.Windows.Forms.ToolStripButton toolStripButtonTrustultimate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}