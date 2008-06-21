using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Emmanuel.Cryptography.GnuPG;
using System.Collections;
using System.Text.RegularExpressions;
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
                public string Username;
                public string userEmail;
                public string userComment;
                public bool valid;
                public Hashtable signs = new Hashtable();
            }
            public ArrayList Items = new ArrayList();
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", false);

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
                            Regex r = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}", RegexOptions.IgnoreCase);
                            Match email = r.Match(thisKeyringUser.User);
                            thisKeyringUser.userEmail = email.Value;
                            r = new Regex(@".*(?=(\s*<))", RegexOptions.IgnoreCase);
                            email = r.Match(thisKeyringUser.User);
                            thisKeyringUser.Username = email.Value;
                            r = new Regex(@"(?<=\().*(?=\))", RegexOptions.IgnoreCase);
                            email = r.Match(thisKeyringUser.User);
                            thisKeyringUser.userComment = email.Value;
                            thisKeyringUser.Generated = DateTime.Parse(thisline[5], culture);
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

        private void Sendkey2()
        {
            KeyMail keyMailWindow = new KeyMail();

            foreach (TreeNode node in keyview.Nodes)
            {
                if (node.Level == 0)
                {
                    TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)node.Tag;
                    keyMailWindow.SendKeyGridView.Rows[keyMailWindow.SendKeyGridView.Rows.Add(false, false, myuser.User)].Tag = node.Tag;
                }
            }
            if (keyMailWindow.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string emails = "";
            string keyIDs = "";
            string users = "";
            foreach (DataGridViewRow row in keyMailWindow.SendKeyGridView.Rows)
            {
                TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)row.Tag;
                if ((bool)row.Cells[0].Value == true)
                {
                    keyIDs += myuser.UserId + " ";
                    users += myuser.User + "\n";
                }
                if ((bool)row.Cells[1].Value == true)
                {
                    emails += myuser.userEmail + "; ";
                }
            }

            if (keyIDs != "")
            {
                string outputText = "";
                string errorText = "";
                // a little trick to workaround to make multiple arguments out of a single line again
                gpg.command = Commands.ShowKey;
                gpg.armor = true;
                gpg.passphrase = "";
                //TheSign.KeyForm.KeyRingData.KeyRingUser myuser = (TheSign.KeyForm.KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                //                gpg.ExecuteCommand("", myuser.UserId, out outputText, out errorText);
                gpg.ExecuteCommandMultiple("", keyIDs, out outputText, out errorText, true);
                if (outputText != "")
                {
                    try
                    {
                        // connect to Outlook
                        Outlook._Application outLookApp = new Outlook.Application();
                        string tempfile = Path.GetTempPath() + "\\theSign.pubkey";
                        StreamWriter fs = new StreamWriter(tempfile);
                        fs.WriteLine(outputText);
                        fs.Close();
                        Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                        actMail.Subject = "TheSign: Some Public Keys of other users";
                        actMail.Body = "The attached .pubkey file contains the public key(s) of the following users:\n\n";
                        actMail.Body = actMail.Body + users;
                        actMail.Body = actMail.Body + "\n\nTo add or update these users in your public key ring, just drag and drop the pubkey attachment into your TheSign Window\n\n";
//                        actMail.Body = actMail.Body + outputText;
                        actMail.To = emails;
                        actMail.Attachments.Add(tempfile, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Display(true);
                        Activate();
                        File.Delete(tempfile);
                    }
                    catch
                    {
                        MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
                    }

                }

            }
            else
            {
                MessageBox.Show("You need to select at least one \"Key\" to send something :-)", "TheSign Hint");
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
                if (testDialog.ShowDialog("Confirm signature from " + myuser.User) == DialogResult.OK)
                {
                    gpg.passphrase = testDialog.passPhraseText.Text;
                    if (!testDialog.storePassPhrase.Checked)
                    {
                        testDialog.passPhraseText.Text = "";
                    } string outputText = "";
                    string errorText = "";
                    gpg.command = Commands.SignKey;
                    gpg.armor = true;
                    gpg.batch = true; //GPG asks for confirmation in stdin if not in batch mode
                    gpg.ExecuteCommand("", myuser.UserId, out outputText, out errorText);
                    if (errorText != "")
                    {
                        MessageBox.Show(errorText, "GPG replies:");

                    }
                    //gpg.batch = true;
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
                    MessageBox.Show( errorText,"GPG replies:");

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

        private void toolStripButtonSend_Click(object sender, EventArgs e)
        {
            Sendkey2();
        }

        private void toolStripSign_Click(object sender, EventArgs e)
        {
            SignKey();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DeleteKey();
        }

    }
}
