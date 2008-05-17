using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Emmanuel.Cryptography.GnuPG;
using System.Text.RegularExpressions;
using TheSign;


namespace SignBrowser
{
    public partial class SignBrowser : Form
    {
        GnuPGWrapper gpg = new GnuPGWrapper();
        XmlDocument xDoc;
        Hashtable authDepartments = new Hashtable();

        class signdata
        {
            public string email;
            public DateTime date;
        }

        public SignBrowser()
        {
            InitializeComponent();
            gpg.homedirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\GnuPG";
            gpg.passphrase = "";
            folderBrowserDialog.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);
            ExportComboBox.SelectedIndex = 0;
            Text = Text + " | " + build.version + "  " + build.buildver;

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            SignGridView.Rows.Clear();
            string[] directorylist = Directory.GetFiles(folderBrowserDialog.SelectedPath);
            processBar.Maximum = directorylist.Length;
            processBar.Minimum = 0;
            processBar.Value = 0;
            foreach (string fileName in directorylist)
            {
                processBar.Value++;
                if (Path.GetExtension(fileName).ToLower() != ".sig")
                {
                    SignGridView.Rows.Add();
                    SignGridView.Rows[SignGridView.RowCount - 1].Cells[0].Value = Path.GetFileName(fileName);
                    if (System.IO.File.Exists(fileName + ".sig"))
                    {
                        signdata[] signs = new signdata[0];
                        string outputText = "";
                        string errorText = "";
                        gpg.command = Commands.Verify;
                        gpg.armor = true;
                        gpg.batch = false; //Somehow the --batch flag makes GPG to not return all signs, just some...
                        gpg.passphrase = "";
                        try
                        {
                            gpg.ExecuteCommand("", fileName + ".sig", out outputText, out errorText);
                        }
                        catch
                        {
                        }
                        signs = EvaluateResult(errorText);
                        gpg.batch = true;
                        string signature = "";
                        foreach (signdata thissign in signs)
                        {
                            signature += thissign + ";";
                        }
                        SignGridView.Rows[SignGridView.RowCount - 1].Cells[2].Value = signature;
                        if (signature != "")
                        {
                            string missdepartments = "";
                            if (checkAuthorities(ref missdepartments, signs))
                            {
                                SignGridView.Rows[SignGridView.RowCount - 1].Cells[1].Value = "Yes";
                            }
                            else
                            {
                                SignGridView.Rows[SignGridView.RowCount - 1].Cells[1].Value = "-";
                            }
                            SignGridView.Rows[SignGridView.RowCount - 1].Cells[3].Value = missdepartments;
                        }
                    }
                    else
                    {
                        SignGridView.Rows[SignGridView.RowCount - 1].Cells[1].Value = "-";
                        SignGridView.Rows[SignGridView.RowCount - 1].Cells[3].Value = "No signature file";

                    }
                }
            }
            GoButton.Enabled = SignGridView.RowCount > 0;
        }

        private signdata[] EvaluateResult(string text)
        {
            ArrayList signs = new ArrayList();
            DateTime lastDate = DateTime.FromBinary(0);
            foreach (string line in text.Split('\n'))
            {
                if (line.ToLower().Contains("signature"))
                {
                    if (line.ToLower().Contains("good"))
                    {
                        Regex r = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}", RegexOptions.IgnoreCase);
                        Match email = r.Match(line);
                        signdata thissign = new signdata();

                        if (lastDate!=DateTime.FromBinary(0))
                        {
                            thissign.email=email.Value;
                            thissign.date = lastDate;
                            signs.Add(thissign);
                            lastDate = DateTime.FromBinary(0);
                        }
                    }

                }
                // gpg: Signature made 02/14/08 08:45:40 
                if (line.ToLower().Contains("gpg: signature made"))
                {
                    Regex r = new Regex(@"\d{2}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}", RegexOptions.IgnoreCase);
                    Match datestring = r.Match(line);
                    lastDate = DateTime.Parse(datestring.Value);
                }
            }
            return (signdata[])signs.ToArray(typeof(signdata));
        }

        private bool checkAuthorities(ref string missings, signdata[] signatures)
        {
            Hashtable foundDepartments = new Hashtable();
            missings = "";
            foreach (signdata thisemail in signatures)
            {
                XmlNodeList emails = xDoc.GetElementsByTagName("email");
                foreach (XmlNode thisnode in emails)
                {
                    try
                    {
                        if (thisnode.InnerText == thisemail.email)
                        {
                            XmlNode parentnode = thisnode.ParentNode.ParentNode; //move up to the department node
                            string department = parentnode.Attributes.GetNamedItem("name").Value; //getting the Name of the department
                            try
                            {
                                foundDepartments[department] = Convert.ToInt32(foundDepartments[department]) + 1;
                            }
                            catch
                            {
                                foundDepartments[department] = 1; ; //that's the first entry
                            }
                        }
                    }
                    catch { }
                }
            }
            //Evaluating the result
            foreach (string department in authDepartments.Keys)
            {
                if (Convert.ToInt32(authDepartments[department]) > Convert.ToInt32(foundDepartments[department]))
                {
                    missings += department + ";";
                }
            }

            return missings == "";
        }

        private void FolderDialogButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                authDepartments.Clear();
                SignGridView.RowCount = 0;
                processBar.Value = 0;
                if (System.IO.File.Exists(folderBrowserDialog.SelectedPath + "\\authorities.xml"))
                {
                    try
                    {
                        xDoc = new XmlDocument();
                        xDoc.Load(folderBrowserDialog.SelectedPath + "\\authorities.xml");
                        XmlNodeList departments = xDoc.GetElementsByTagName("department");
                        foreach (XmlNode thisnode in departments)
                        {
                            try
                            {
                                string department = thisnode.Attributes.GetNamedItem("name").Value;
                                try
                                {
                                    authDepartments[department] = Convert.ToInt32(thisnode.Attributes.GetNamedItem("needed").Value);
                                }
                                catch
                                {
                                    authDepartments[department] = 9999; //makes this department unsignable ;-)
                                }
                            }
                            catch { }
                        }
                        StartButton.Enabled = true;
                        StartButton.Text = "Start";
                    }
                    catch
                    {
                        StartButton.Enabled = false;
                        StartButton.Text = "Error in authority file ...";
                    }
                }
                else
                {
                    StartButton.Enabled = false;
                    StartButton.Text = "No authority file found...";
                }
            }
        }

        private void GoButton_Click(object sender, EventArgs e)
        {

            switch (ExportComboBox.SelectedIndex)
            {
                case 0:
                    string clip = "";
                    foreach (DataGridViewRow row in SignGridView.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null)
                            {
                                clip += cell.Value.ToString() + "\t";
                            }
                            else
                            {
                                clip += "\t";
                            }
                        }
                        clip += "\r\n";
                    }
                    Clipboard.SetText(clip);
                    break;
                default:
                    break;
            }
        }

    }
}
