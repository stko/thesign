using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emmanuel.Cryptography.GnuPG;
using System.Collections;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace TheSign
{
    public partial class KeyForm : Form
    {
        private GnuPGWrapper gpg;

        private PassphraseForm testDialog;

        public class KeyRingData
        {
            public class KeyRingUser
            {
                public string UserId;
                public DateTime Expires;
                public DateTime Generated;
                public string Fingerprint;
                public string User;
                public bool valid;
                public Hashtable signs = new Hashtable();
            }
            public ArrayList Items = new ArrayList();
            public KeyRingData(bool seckeyring, GnuPGWrapper gpg)
            {
                string outputText = "";
                string errorText = "";
                if (seckeyring)
                {
                    gpg.command = Commands.Seckey;
                    gpg.armor = true;
                    gpg.passphrase = "";
                    gpg.ExecuteCommand("", "", out outputText, out errorText);
                }
                else
                {
                    gpg.command = Commands.List;
                    gpg.armor = true;
                    gpg.passphrase = "";
                    gpg.ExecuteCommand("", "", out outputText, out errorText);
                }
                KeyRingUser thisKeyringUser = null;
                foreach (string line in (outputText.Split('\n')))
                {
                    string[] thisline = line.Split(':');
                    try
                    {
                        if (thisline[0] == "sec" || thisline[0] == "pub")
                        {
                            if (thisKeyringUser != null)
                            {
                                Items.Add(thisKeyringUser);
                            }

                            thisKeyringUser = new KeyRingUser();
                            thisKeyringUser.UserId = thisline[4];
                            thisKeyringUser.User = thisline[9];
                            thisKeyringUser.Generated = DateTime.Parse(thisline[5]);
                            if (thisline[6] != "")
                            {
                                thisKeyringUser.Expires = DateTime.Parse(thisline[6]);
                                thisKeyringUser.valid = (thisKeyringUser.Expires.Ticks == 0 || thisKeyringUser.Expires.Ticks > DateTime.Now.Ticks);
                            }
                            else
                            {
                                thisKeyringUser.valid = true;
                            }
                        }
                        if (thisline[0] == "fpr")
                        {
                            thisKeyringUser.Fingerprint = thisline[9];
                        }
                        if (thisline[0] == "sig")
                        {
                            if (thisKeyringUser != null)
                            {
                                thisKeyringUser.signs[thisline[4]] = thisline[9];
                            }
                        }
                    }
                    catch
                    { }
                }
                if (thisKeyringUser != null)
                {
                    Items.Add(thisKeyringUser);
                }


            }
        }

        private Hashtable keys = new Hashtable();

        public KeyForm(GnuPGWrapper gpg, PassphraseForm passform)
        {
            InitializeComponent();
            this.gpg = gpg;
            this.testDialog = passform;
            LoadDB();
            KeyForm.KeyRingData privateKey = new KeyForm.KeyRingData(true, gpg);
        }

        private bool LoadDB()
        {
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.loadTrust;
            gpg.armor = true;
            gpg.passphrase = "";

            int i = 0;
            gpg.ExecuteCommand("", "", out outputText, out errorText);
            foreach (string line in (outputText.Split('\n')))
            {
                if (i > 1) // skip the first 2 lines
                {
                    string[] thisline = line.Trim().Split(':');
                    try
                    {
                        keys[thisline[0]] = Convert.ToInt32(thisline[1], 10);
                    }
                    catch
                    { }
                }
                i++;
            }
            return keys.Count > 0;
        }

        private bool SaveDB()
        {
            if (keys.Count > 0)
            {
                string outputText = "";
                string errorText = "";
                string keytext = "";
                foreach (string key in keys.Keys)
                {
                    keytext += key + ":" + keys[key].ToString() + "\n";
                }
                gpg.command = Commands.writeTrust;
                gpg.armor = true;
                gpg.passphrase = "";
                gpg.ExecuteCommand(keytext, "", out outputText, out errorText);

            }
            return true;
        }

        private int getTrustLevel(string fingerprint)
        {
            int trustlevel = 0;
            try
            {
                trustlevel = (int)keys[fingerprint] - 2;
            }
            catch
            {
            }
            return trustlevel;
        }

        private void buildTree()
        {
            LoadDB();
            KeyRingData publicKey = new KeyForm.KeyRingData(false, gpg);
            keyview.Nodes.Clear();
            foreach (KeyRingData.KeyRingUser user in publicKey.Items)
            {
                TreeNode thisnode = keyview.Nodes.Add(user.User);
                thisnode.Tag = user;
                thisnode.ImageIndex = getTrustLevel(user.Fingerprint);
                thisnode.SelectedImageIndex = thisnode.ImageIndex;
                thisnode.ToolTipText = "fingerprint:" + user.Fingerprint;
                foreach (string sig in user.signs.Keys)
                {
                    //                    TreeNode signode = thisnode.Nodes.Add(user.signs[sig].ToString, sig);
                    TreeNode signode = thisnode.Nodes.Add(user.signs[sig].ToString());
                    signode.ImageIndex = 5;
                    signode.SelectedImageIndex = 5;
                    signode.Tag = sig;
                }
            }
        }

        private void KeyForm_Shown(object sender, EventArgs e)
        {
            buildTree();
        }

        private void Sendkey()
        {
            if (keyview.SelectedNode != null)
            {
                string outputText = "";
                string errorText = "";
                gpg.command = Commands.ShowKey;
                gpg.armor = true;
                gpg.passphrase = "";
                TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                gpg.ExecuteCommand("", myuser.UserId, out outputText, out errorText);
                if (outputText != "")
                {
                    try
                    {
                        // connect to Outllok
                        Outlook._Application outLookApp = new Outlook.Application();

                        Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                        actMail.Subject = "TheSign: Public Key of " + keyview.SelectedNode.Text;
                        actMail.Body = outputText;
                        actMail.Display(true);
                    }
                    catch
                    {
                        MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
                    }

                }

            }
        }

        private void ReadKeyFromMail()
        {
            try
            {
                // connect to Outlook
                Outlook._Application outLookApp = new Outlook.Application();
                // search for the active element
                Outlook._Explorer myExplorer = outLookApp.ActiveExplorer();
                // is one item selected?
                if (myExplorer.Selection.Count == 1)
                {
                    //is it a Mail?
                    if (myExplorer.Selection[1] is Outlook.MailItem)
                    {
                        // then reply it
                        Outlook.MailItem actMail = (Outlook.MailItem)myExplorer.Selection[1];
                        string key = actMail.Body;
                        if (key != "")
                        {

                            string outputText = "";
                            string errorText = "";
                            gpg.command = Commands.AddKey;
                            gpg.armor = true;
                            gpg.passphrase = "";
                            try
                            {
                                gpg.ExecuteCommand(key, "", out outputText, out errorText);
                            }
                            catch
                            { }
                            if (outputText != "")
                            {
                                MessageBox.Show("GPG replies oText:\n" + outputText);
                                buildTree();
                            }
                            else
                            {
                                MessageBox.Show("GPG replies eText:\n" + errorText);
                                buildTree();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Sorry, no text found in Mail", "TheSign Error");

                        }

                    }
                    else
                    {
                        MessageBox.Show("Sorry, the Item in Outlook seems no mail..", "TheSign Error");

                    }

                }
                else
                {
                    MessageBox.Show("Sorry, there's more as one item selected in Outlook", "TheSign Error");

                }
            }
            catch
            {
                MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
            }

        }

        private void SignKey()
        {
            if (keyview.SelectedNode != null)
            {
                if (MessageBox.Show("You must not sign a key without being 100% sure\nthat the key belongs to the right person!\n\nDid you checked the Fingerprint against the\nhand signed Certificate & verified the\ncertificate with the assumed owner\ne.g. via phone?\n", "Security Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    return;
                }
                TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                if (testDialog.ShowDialog("Confirm signature from "+myuser.User) == DialogResult.OK)
                {
                    gpg.passphrase = testDialog.passPhraseText.Text;
                    if (!testDialog.storePassPhrase.Checked)
                    {
                        testDialog.passPhraseText.Text = "";
                    } string outputText = "";
                    string errorText = "";
                    gpg.command = Commands.SignKey;
                    gpg.armor = true;
                    gpg.ExecuteCommand("", myuser.UserId, out outputText, out errorText);
                    if (errorText != "")
                    {
                        MessageBox.Show("GPG replies:\n" + errorText);

                    }
                    buildTree();
                }
            }

        }

        private void DeleteKey()
        {
            if (keyview.SelectedNode != null)
            {
                TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                if (MessageBox.Show("Do you really want to delete\n\n" + myuser.User + "\n\nfrom your Keylist?\nThis Deletion can't be reversed!", "Security Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    return;
                }
                string outputText = "";
                string errorText = "";
                gpg.command = Commands.DelKey;
                gpg.armor = true;
                gpg.ExecuteCommand("", myuser.UserId, out outputText, out errorText);
                if (errorText != "")
                {
                    MessageBox.Show("GPG replies:\n" + errorText);

                }
                buildTree();
            }

        }

        private void keyview_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (keyview.SelectedNode == null || keyview.SelectedNode.Parent != null)
            {
                KeyStrip.Enabled = false;
            }
            else
            {
                KeyStrip.Enabled = true;
            }
        }

        private void toolStripButtonTrust_dont_know_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem mybutton = (ToolStripItem)sender;
                int newtrustlevel = Convert.ToInt32(mybutton.Tag);
                TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                keys[myuser.Fingerprint] = newtrustlevel + 1;
                SaveDB();
                buildTree();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Sendkey();
        }

        private void toolStripSign_Click(object sender, EventArgs e)
        {
            SignKey();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DeleteKey();
        }

        private void toolStripButtonReadMail_Click(object sender, EventArgs e)
        {
            ReadKeyFromMail();
        }


    }
}
