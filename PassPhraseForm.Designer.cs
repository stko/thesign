namespace TheSign
{
    partial class PassphraseForm
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
            this.passPhraseText = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.storePassPhrase = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // passPhraseText
            // 
            this.passPhraseText.Location = new System.Drawing.Point(20, 25);
            this.passPhraseText.Name = "passPhraseText";
            this.passPhraseText.Size = new System.Drawing.Size(388, 20);
            this.passPhraseText.TabIndex = 0;
            this.passPhraseText.UseSystemPasswordChar = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(297, 74);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(111, 27);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Sign";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // storePassPhrase
            // 
            this.storePassPhrase.AutoSize = true;
            this.storePassPhrase.Location = new System.Drawing.Point(20, 51);
            this.storePassPhrase.Name = "storePassPhrase";
            this.storePassPhrase.Size = new System.Drawing.Size(208, 17);
            this.storePassPhrase.TabIndex = 1;
            this.storePassPhrase.Text = "Remember PassPhrase for this session";
            this.storePassPhrase.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(135, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Please enter your PassPhrase:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(20, 74);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(118, 26);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Don\'t Sign this";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // PassphraseForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 112);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.storePassPhrase);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.passPhraseText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PassphraseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please enter your PassPhrase";
            this.Activated += new System.EventHandler(this.PassphraseForm_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox passPhraseText;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.CheckBox storePassPhrase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
    }
}