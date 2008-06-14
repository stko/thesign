namespace TheSign
{
    partial class KeyMail
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
            this.SendKeyGridView = new System.Windows.Forms.DataGridView();
            this.StartButton = new System.Windows.Forms.Button();
            this.key = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.To = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.SendKeyGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SendKeyGridView
            // 
            this.SendKeyGridView.AllowUserToAddRows = false;
            this.SendKeyGridView.AllowUserToDeleteRows = false;
            this.SendKeyGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SendKeyGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SendKeyGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.key,
            this.To,
            this.Email});
            this.SendKeyGridView.Location = new System.Drawing.Point(12, 12);
            this.SendKeyGridView.Name = "SendKeyGridView";
            this.SendKeyGridView.Size = new System.Drawing.Size(672, 209);
            this.SendKeyGridView.TabIndex = 0;
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.StartButton.Location = new System.Drawing.Point(12, 227);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(672, 27);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Create Mail";
            this.StartButton.UseVisualStyleBackColor = true;
            // 
            // key
            // 
            this.key.HeaderText = "Key";
            this.key.Name = "key";
            // 
            // To
            // 
            this.To.HeaderText = "Send to";
            this.To.Name = "To";
            // 
            // Email
            // 
            this.Email.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Email.HeaderText = "User";
            this.Email.Name = "Email";
            this.Email.ReadOnly = true;
            // 
            // KeyMail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 266);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.SendKeyGridView);
            this.Name = "KeyMail";
            this.Text = "TheSign: Send Keys as Email";
            ((System.ComponentModel.ISupportInitialize)(this.SendKeyGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        public System.Windows.Forms.DataGridView SendKeyGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn key;
        private System.Windows.Forms.DataGridViewCheckBoxColumn To;
        private System.Windows.Forms.DataGridViewTextBoxColumn Email;
    }
}