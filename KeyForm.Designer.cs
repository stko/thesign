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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.keyview = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sendbutton = new System.Windows.Forms.Button();
            this.readbutton = new System.Windows.Forms.Button();
            this.deletebutton = new System.Windows.Forms.Button();
            this.trustbutton = new System.Windows.Forms.Button();
            this.Signbutton = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.keyview);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(573, 284);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Available Keys:";
            // 
            // keyview
            // 
            this.keyview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyview.Location = new System.Drawing.Point(3, 16);
            this.keyview.Name = "keyview";
            this.keyview.Size = new System.Drawing.Size(567, 265);
            this.keyview.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.readbutton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.deletebutton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.trustbutton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.Signbutton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.sendbutton, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 300);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 76);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // sendbutton
            // 
            this.sendbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.sendbutton, 3);
            this.sendbutton.Location = new System.Drawing.Point(1, 1);
            this.sendbutton.Margin = new System.Windows.Forms.Padding(1);
            this.sendbutton.Name = "sendbutton";
            this.sendbutton.Size = new System.Drawing.Size(568, 23);
            this.sendbutton.TabIndex = 0;
            this.sendbutton.Text = "Send Key via Email...";
            this.sendbutton.UseVisualStyleBackColor = true;
            this.sendbutton.Click += new System.EventHandler(this.sendbutton_Click);
            // 
            // readbutton
            // 
            this.readbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.readbutton, 3);
            this.readbutton.Location = new System.Drawing.Point(1, 26);
            this.readbutton.Margin = new System.Windows.Forms.Padding(1);
            this.readbutton.Name = "readbutton";
            this.readbutton.Size = new System.Drawing.Size(568, 23);
            this.readbutton.TabIndex = 1;
            this.readbutton.Text = "Read Key from actual selected Email";
            this.readbutton.UseVisualStyleBackColor = true;
            this.readbutton.Click += new System.EventHandler(this.readbutton_Click);
            // 
            // deletebutton
            // 
            this.deletebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.deletebutton.Enabled = false;
            this.deletebutton.Location = new System.Drawing.Point(1, 51);
            this.deletebutton.Margin = new System.Windows.Forms.Padding(1);
            this.deletebutton.Name = "deletebutton";
            this.deletebutton.Size = new System.Drawing.Size(187, 24);
            this.deletebutton.TabIndex = 2;
            this.deletebutton.Text = "Delete Key...";
            this.deletebutton.UseVisualStyleBackColor = true;
            // 
            // trustbutton
            // 
            this.trustbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trustbutton.Enabled = false;
            this.trustbutton.Location = new System.Drawing.Point(190, 51);
            this.trustbutton.Margin = new System.Windows.Forms.Padding(1);
            this.trustbutton.Name = "trustbutton";
            this.trustbutton.Size = new System.Drawing.Size(187, 24);
            this.trustbutton.TabIndex = 3;
            this.trustbutton.Text = "Change Level of Trust...";
            this.trustbutton.UseVisualStyleBackColor = true;
            // 
            // Signbutton
            // 
            this.Signbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Signbutton.Location = new System.Drawing.Point(379, 51);
            this.Signbutton.Margin = new System.Windows.Forms.Padding(1);
            this.Signbutton.Name = "Signbutton";
            this.Signbutton.Size = new System.Drawing.Size(190, 24);
            this.Signbutton.TabIndex = 4;
            this.Signbutton.Text = "Sign Key...";
            this.Signbutton.UseVisualStyleBackColor = true;
            this.Signbutton.Click += new System.EventHandler(this.Signbutton_Click);
            // 
            // KeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 386);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBox3);
            this.Name = "KeyForm";
            this.Text = "TheSign- Key List Window";
            this.Shown += new System.EventHandler(this.KeyForm_Shown);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TreeView keyview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button sendbutton;
        private System.Windows.Forms.Button readbutton;
        private System.Windows.Forms.Button deletebutton;
        private System.Windows.Forms.Button trustbutton;
        private System.Windows.Forms.Button Signbutton;
    }
}