using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheSign
{
    public partial class PassphraseForm : Form
    {
        public  PassphraseForm()
        {
            InitializeComponent();
        }

        public System.Windows.Forms.DialogResult ShowDialog(string title)
        {
            Text=title;
            return base.ShowDialog();
            
        }

        private void PassphraseForm_Activated(object sender, EventArgs e)
        {
            passPhraseText.Focus();
        }
    }
}
