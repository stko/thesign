﻿using System;
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
                            thisKeyringUser.Expires = DateTime.Parse(thisline[6]);
                            thisKeyringUser.valid = (thisKeyringUser.Expires.Ticks == 0 || thisKeyringUser.Expires.Ticks > DateTime.Now.Ticks);
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
        public KeyForm(GnuPGWrapper gpg,PassphraseForm passform)
        {
            InitializeComponent();
            this.gpg = gpg;
            this.testDialog = passform;
            LoadDB();
            SaveDB();
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
        private void buildTree()
        {
            KeyRingData publicKey = new KeyForm.KeyRingData(false, gpg);
            keyview.Nodes.Clear();
            foreach (KeyRingData.KeyRingUser user in publicKey.Items)
            {
                TreeNode thisnode = keyview.Nodes.Add(user.User + " (" + user.UserId + ")");
                thisnode.Tag = user.UserId;
                foreach (string sig in user.signs.Keys)
                {
                    TreeNode signode = thisnode.Nodes.Add(user.signs[sig] + " (" + sig + ")");
                    signode.Tag = sig;
                }
            }
        }
        private void KeyForm_Shown(object sender, EventArgs e)
        {
            buildTree();
        }

        private void sendbutton_Click(object sender, EventArgs e)
        {
            if (keyview.SelectedNode != null)
            {
                MessageBox.Show(keyview.SelectedNode.Tag.ToString());
                string outputText = "";
                string errorText = "";
                gpg.command = Commands.ShowKey;
                gpg.armor = true;
                gpg.passphrase = "";
                gpg.ExecuteCommand("", keyview.SelectedNode.Tag.ToString(), out outputText, out errorText);
                if (outputText != "")
                {
                    MessageBox.Show(outputText);
                    try
                    {
                        // connect to Outllok
                        Outlook._Application outLookApp = new Outlook.Application();

                        Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                        actMail.Subject = "Public Key of " + keyview.SelectedNode.Text;
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

        private void readbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // connect to Outllok
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
                            gpg.ExecuteCommand(key, "", out outputText, out errorText);
                            if (outputText != "")
                            {
                                MessageBox.Show("GPG replies:\n"+outputText);
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error: No connection to Outlook found..","TheSign Error");
            }

        }

        private void Signbutton_Click(object sender, EventArgs e)
        {
            if (keyview.SelectedNode != null)
            {
                if (testDialog.ShowDialog() == DialogResult.OK)
                {
                    gpg.passphrase = testDialog.passPhraseText.Text;
                    if (!testDialog.storePassPhrase.Checked)
                    {
                        testDialog.passPhraseText.Text = "";
                    } string outputText = "";
                    string errorText = "";
                    gpg.command = Commands.SignKey;
                    gpg.armor = true;
                    gpg.verbose = VerboseLevel.VeryVerbose;
                    gpg.ExecuteCommand("", keyview.SelectedNode.Tag.ToString(), out outputText, out errorText);
                    if (errorText != "")
                    {
                        MessageBox.Show("GPG replies:\n" + errorText);

                    }
                }
            }

        }


    }
}
