using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emmanuel.Cryptography.GnuPG;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml;
using System.IO;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections;
using Microsoft.Win32;


namespace TheSign
{
    public partial class Main : Form
    {

        GnuPGWrapper gpg = new GnuPGWrapper();
        PassphraseForm testDialog = new PassphraseForm();
//        KeyForm keywindow;
        AboutBox aboutwindow = new AboutBox();
        string lastcheckedFile = "";
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
        XmlDocument xDoc;
        Hashtable authDepartments = new Hashtable();
        IFormatProvider culture = new System.Globalization.CultureInfo("en-US", false);

        class signdata
        {
            public string email,
            Username,
                userComment;
            public DateTime date;
        }



        public Main()
        {
            InitializeComponent();
            openFileDialog.InitialDirectory = (string)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "FileDir", Path.GetDirectoryName(Application.ExecutablePath));
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "ExePath", Path.GetDirectoryName(Application.ExecutablePath));
            gpg.homedirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\GnuPG";
            gpg.passphrase = "signtest";
            //keywindow = new KeyForm(gpg, testDialog);
            LoadDB();
            buildTree();
 
            KeyRingData privateKey = new KeyRingData(true, gpg);
            int i = 0;
            bool endflag = false;
            KeyRingData.KeyRingUser thisitem = null;
            while (i < privateKey.Items.Count && !endflag)
            {
                thisitem = (KeyRingData.KeyRingUser)privateKey.Items[i];
                if (thisitem.valid)
                {
                    endflag = true;
                }
                else
                {
                    i++;
                }
            }
            if (endflag)
            {
                gpg.originator = thisitem.UserId;
                Text = thisitem.User + " | " + Text + " | " + build.version + "  " + build.buildver;
            }
            else
            {
                MessageBox.Show("No valid Userid found - Installation incomplete?");
            }

            // Set some parameters from on Web.Config file

            folderBrowserDialog.SelectedPath = (string)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "BrowseDir", Path.GetDirectoryName(Application.ExecutablePath));
            checkfolder(folderBrowserDialog.SelectedPath);
            ExportComboBox.SelectedIndex = 0;

        }
        ~Main()
        {
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "FileDir", openFileDialog.InitialDirectory);

        }

        private void Output_DragDrop(object sender, DragEventArgs e)
        {

            string[] fileNames = null;


            this.Activate();
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                {
                    clearWindow();
                    fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                    // handle each file passed as needed
                    foreach (string fileName in fileNames)
                    {
                        HandleFile(fileName, false);
                    }
                }
                else if (e.Data.GetDataPresent("FileGroupDescriptor"))
                {
                    clearWindow();
                    //
                    // the first step here is to get the filename of the attachment and
                    //   build a full-path name so we can store it in the temporary folder
                    //

                    // set up to obtain the FileGroupDescriptor and extract the file name
                    Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor");
                    byte[] fileGroupDescriptor = new byte[512];
                    theStream.Read(fileGroupDescriptor, 0, 512);
                    // used to build the filename from the FileGroupDescriptor block
                    StringBuilder fileName = new StringBuilder("");
                    // this trick gets the filename of the passed attached file
                    for (int i = 76; fileGroupDescriptor[i] != 0; i++)
                    { fileName.Append(Convert.ToChar(fileGroupDescriptor[i])); }
                    theStream.Close();
                    string path = Path.GetTempPath();  // put the zip file into the temp directory
                    //                        string path = Path.GetDirectoryName(Application.ExecutablePath)+"\\SignedFiles\\";  // put the zip file into the temp directory
                    string theFile = path + fileName.ToString();  // create the full-path name

                    //
                    // Second step:  we have the file name.  Now we need to get the actual raw
                    //    data for the attached file and copy it to disk so we work on it.
                    //

                    // get the actual raw file into memory
                    MemoryStream ms = (MemoryStream)e.Data.GetData("FileContents", true);
                    // allocate enough bytes to hold the raw data
                    byte[] fileBytes = new byte[ms.Length];
                    // set starting position at first byte and read in the raw data
                    ms.Position = 0;
                    ms.Read(fileBytes, 0, (int)ms.Length);
                    // create a file and save the raw zip file to it
                    FileStream fs = new FileStream(theFile, FileMode.Create);
                    fs.Write(fileBytes, 0, (int)fileBytes.Length);

                    fs.Close();	// close the file

                    FileInfo tempFile = new FileInfo(theFile);

                    // always good to make sure we actually created the file
                    if (tempFile.Exists == true)
                    {
                        // for now, just delete what we created
                        Output.Text = Output.Text + "\r\nMail File: " + theFile;
                        HandleFile(theFile, true);

                        tempFile.Delete();
                    }
                    else
                    {
                        //Trace.WriteLine("File was not created!"); 
                        Output.Text = Output.Text + "\r\nError: " + "File was not created!";
                    }
                }
            }
            catch (Exception ex)
            {
                Output.Text = Output.Text + "\r\nError: " + ex.Message;
                //Trace.WriteLine("Error in DragDrop function: " + ex.Message);

                // don't use MessageBox here - Outlook or Explorer is waiting !
            }
        }

        private void SendFileandSig()
        {
            if (lastcheckedFile != "")
            {
                if (Path.GetExtension(lastcheckedFile).ToLower() == ".sig")
                {
                    lastcheckedFile = Path.GetDirectoryName(lastcheckedFile) +"\\"+ Path.GetFileNameWithoutExtension(lastcheckedFile);
                }
                try
                {
                    // connect to Outllok
                    Outlook._Application outLookApp = new Outlook.Application();

                    Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);

                    actMail.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    actMail.Subject = "File & Sig of  " + Path.GetFileName(lastcheckedFile) + " [TheSign]";
                    actMail.HTMLBody = "<html><body><p><i>Please add a few friendly words to the receipient here :-)</i></p><span style=\"font-size:0.6em\">(made by <a href=\"http://www.koehlers.de/wiki/doku.php?id=thesign:index\">TheSign</a>)</span>";
                    try
                    {
                        actMail.Attachments.Add(lastcheckedFile, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Attachments.Add(lastcheckedFile + ".sig", Type.Missing, Type.Missing, Type.Missing);
                        actMail.Display(true);
                    }
                    catch
                    {
                        MessageBox.Show("Error: Can't attach file\n"+lastcheckedFile, "TheSign Error");
                    }


                }
                catch
                {
                    MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
                }

            }
        }

        public void sendBugReport(string title, params object[] variables)
        {
            if (MessageBox.Show("TheSign just discovered an error\nIs it ok to send a error report to steffen@koehlers.de?", "TheSign Error detected", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            string errorlist = "Error:\n" + title + "\n\nVariables:\n\n";
            int i = 1;
            foreach (object myobj in variables)
            {
                errorlist = errorlist + "var_" + i.ToString() + ":" + myobj.ToString() + "\r\n";
                i++;
            }
            try
            {
                // connect to Outllok
                Outlook._Application outLookApp = new Outlook.Application();

                Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                actMail.To = "steffen@koehlers.de";
                actMail.Subject = "TheSign Error Report";
                actMail.Body = errorlist;
                actMail.Display(true);

            }
            catch
            {
                MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
            }

        }

        private void SendSignaturesOnly()
        {
            string text = Output.Text;
            if (text != "")
            {

                string outputText = "";
                string errorText = "";
                if (testDialog.ShowDialog("Sign last output text") == DialogResult.OK)
                {
                    gpg.passphrase = testDialog.passPhraseText.Text;
                    if (!testDialog.storePassPhrase.Checked)
                    {
                        testDialog.passPhraseText.Text = "";
                    }
                    gpg.command = Commands.clearSign;
                    gpg.armor = true;
                    gpg.ExecuteCommand(text, "", out outputText, out errorText);
                    if (outputText != "")
                    {
                        try
                        {
                            // connect to Outllok
                            Outlook._Application outLookApp = new Outlook.Application();

                            Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                            actMail.Subject = "TheSign: Confirmation of Signatures";
                            actMail.Body = "This is a mail generated by the TheSign (http://www.koehlers.de/wiki/doku.php?id=thesign:index)\n\nHere's the confirmation that " + Path.GetFileName(lastcheckedFile) + " is signed as follows\n" + outputText;
                            actMail.Display(true);

                        }
                        catch
                        {
                            MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
                        }
                    }
                }
            }
        }

        private void checkEmailValidity()
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
                        string InlineSignature = actMail.Body;
                        this.Activate();
                        if (InlineSignature != "")
                        {

                            string outputText = "";
                            string errorText = "";
                            gpg.command = Commands.Verify;
                            gpg.armor = true;
                            gpg.passphrase = "";
                            try
                            {
                                gpg.ExecuteCommand(InlineSignature, "", out outputText, out errorText);
                            }
                            catch
                            { }
                            if (outputText != "")
                            {
                                MessageBox.Show(outputText, "GPG replies (oText):");
                            }
                            else
                            {
                                MessageBox.Show(errorText, "GPG replies (eText):");
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

        private void HandleFile(string fileName, Boolean isMail)
        {
            // do what you are going to do with each filename
            if (Path.GetExtension(fileName) == ".sig")
            {
                if (isMail)
                {
                    openFileDialog.Title = "Select original file to add the sign to:";
                    openFileDialog.FileName = Path.GetFullPath(openFileDialog.InitialDirectory) + "\\" + Path.GetFileName((Path.GetFileNameWithoutExtension(fileName)));
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        openFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                        Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "FileDir", openFileDialog.InitialDirectory);
                        string targetFileName = openFileDialog.FileName;
                        while (Path.GetExtension(targetFileName).ToLower() == ".sig")
                        {
                            targetFileName = Path.GetFileNameWithoutExtension(targetFileName);
                        }
                        lastcheckedFile = targetFileName;
                        targetFileName = MoveIntoFile(targetFileName, ".sig", fileName, true, true, true);
                        Output.Text += "\r\nSignature saved";
                        cleanupSigfile(targetFileName);
                        CheckSig(targetFileName, false);
                        toolStripButtonSendFileandSig.Enabled = true;
                        toolStripButtonSendSignaturesOnly.Enabled = true;
                    }
                }
                else
                {
                    CheckSig(fileName, true);
                    lastcheckedFile = fileName;
                    toolStripButtonSendFileandSig.Enabled = true;
                    toolStripButtonSendSignaturesOnly.Enabled = true;
                }
            }
            else if (Path.GetExtension(fileName) == ".pubkey")
            {
                importKey(fileName);
                lastcheckedFile = "";
                toolStripButtonSendFileandSig.Enabled = false;
                toolStripButtonSendSignaturesOnly.Enabled = false;

            }
            else
            {
                if (isMail)
                {
                    fileName = MoveIntoFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\SignedFiles\\" + Path.GetFileName(fileName), "", fileName, true, false, false);
                    if (fileName != "")
                    {
                        SignFile(fileName);
                        ReplyMail(" [TheSign]", "<html><body><p><i>Please add a few friendly words to the receipient here :-)</i></p><span style=\"font-size:0.6em\">(made by <a href=\"http://www.koehlers.de/wiki/doku.php?id=thesign:index\">TheSign</a>)</span>", fileName + ".sig");
                        lastcheckedFile = fileName;
                        toolStripButtonSendFileandSig.Enabled = true;
                        toolStripButtonSendSignaturesOnly.Enabled = true;

                    }
                }
                else
                {
                    SignFile(fileName);
                    lastcheckedFile = fileName;
                    toolStripButtonSendFileandSig.Enabled = true;
                    toolStripButtonSendSignaturesOnly.Enabled = true;

                }
                if (fileName != "")
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                    Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "FileDir", openFileDialog.InitialDirectory);
                }
            }
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.

            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        private string MoveIntoFile(string fileName, string ext, string input, bool isFile, bool Append, bool overwrite)
        {
            fileName += ext;
            if (isFile)
            {
                if (File.Exists(fileName) && !overwrite)
                {
                    if (!FileCompare(fileName, input))
                    {
                        if (MessageBox.Show("The file " + Path.GetFileName(fileName) + " already exits,\nbut it's different!\nDo you really want to overwite?", "Ok to overwrite?", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        {
                            return "";
                        }
                    }

                }
                int bytesread;
                byte[] buffer = new byte[1024];
                Stream inputStream;
                try
                {
                    inputStream = File.OpenRead(input);
                }
                catch
                {
                    return "";
                }

                Stream fs;
                try
                {
                    // Remove read only first

                    if (File.Exists(fileName))
                    {
                        File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
                    }

                    if (Append)
                    {
                        fs = File.Open(fileName, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        fs = File.Create(fileName);
                    }
                }
                catch
                {
                    inputStream.Close();
                    return "";
                }
                while ((bytesread = inputStream.Read(buffer, 0, 1024)) > 0)
                {
                    fs.Write(buffer, 0, bytesread);
                }
                inputStream.Close();
                fs.Close();
                File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);

            }
            else
            {
                if (File.Exists(fileName))
                {
                    File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
                }
                StreamWriter fs = new StreamWriter(fileName, Append);
                fs.Write(input);
                fs.Close();
                File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);
            }
            return fileName;
        }

        private int ReplyMail(String subject, String body, String attachmentName)
        {
            try
            {
                // connect to Outllok
                Outlook._Application outLookApp = new Outlook.Application();
                //http://tangiblesoftwaresolutions.com/Articles/CSharp%20Equivalent%20to%20VB%20CreateObject.htm
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
                        Outlook.MailItem ReplyMail = actMail.Reply();
                        ReplyMail.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                        ReplyMail.Subject = ReplyMail.Subject + subject;
                        ReplyMail.HTMLBody = body + ReplyMail.HTMLBody;
                        //                        ReplyMail.Body = body + ReplyMail.Body;
                        ReplyMail.Attachments.Add(attachmentName, Type.Missing, Type.Missing, Type.Missing);
                        ReplyMail.Display(true);
                    }
                }
            }
            catch
            {
                return 1;
            }
            return 0;
        }

        private void importKey(String fileName)
        {
            clearWindow();
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Import;
            gpg.armor = true;
            Output.Text = Output.Text + "\r\nImport " + Path.GetFileName(fileName) + " ...";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", fileName, out outputText, out errorText);
            Output.Text = Output.Text + Path.GetFileName(fileName) + " Imported:\r\n" + errorText;
        }

        private void CheckSig(String fileName, bool clearfirst)
        {
            if (clearfirst)
            {
                clearWindow();
            }
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Verify;
            gpg.armor = true;
            gpg.batch = false; //Somehow the --batch flag makes GPG to not return all signs, just some...
            Output.Text = Output.Text + "\r\nChecking " + Path.GetFileName(fileName) + " ...";
            gpg.passphrase = "";
            try
            {
                gpg.ExecuteCommand("", fileName, out outputText, out errorText);
            }
            catch
            {
            }
            gpg.batch = true;
            Output.Text = Output.Text + Path.GetFileName(fileName) + " checked:\r\n" + errorText;
        }

        private void SignFile(String fileName)
        {
            clearWindow();
            string outputText = "";
            string errorText = "";
            if (testDialog.ShowDialog("Sign file: " + Path.GetFileName(fileName)) == DialogResult.OK)
            {
                gpg.passphrase = testDialog.passPhraseText.Text;
                if (!testDialog.storePassPhrase.Checked)
                {
                    testDialog.passPhraseText.Text = "";
                }
                gpg.command = Commands.detachsign;
                gpg.armor = true;
                Output.Text = Output.Text + "\r\nSigning " + Path.GetFileName(fileName) + " ...";
                gpg.ExecuteCommand("", fileName, out outputText, out errorText);
                if (errorText == "")
                {
                    MoveIntoFile(fileName, ".sig", outputText, false, true, false);
                    File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);

                    Output.Text = Output.Text + "\r\nSuccess: " + Path.GetFileName(fileName) + " signed\r\n";
                    CheckSig(fileName + ".sig", false);
                }

            }
            else
            {
                Output.Text = Output.Text + "\r\nCanceled";

            }
        }

        private void Output_DragEnter(object sender, DragEventArgs e)
        {
            // for this program, we allow a file to be dropped from Explorer
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            { e.Effect = DragDropEffects.Copy; }
            //    or this tells us if it is an Outlook attachment drop
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            { e.Effect = DragDropEffects.Copy; }
            //    or none of the above
            else
            { e.Effect = DragDropEffects.None; }

        }

        private void Menu_Quit(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void clearWindow()
        {
            Output.Text = "";
        }

        private void Menu_printCertificate(object sender, EventArgs e)
        {
            clearWindow();
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Fingerprint;
            gpg.armor = true;
            Output.Text = Output.Text + "\r\nGenerate Certificate sheet:\r\n";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", "", out outputText, out errorText);
            if (outputText == "")
            {
                Output.Text = Output.Text + "Errortext:" + errorText + "Done\r\n";
                return;
            }
            gpg.command = Commands.ShowKey;
            gpg.armor = true;
            string keystring;
            Output.Text = Output.Text + "\r\nExtract own public key:\r\n";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", gpg.originator, out keystring, out errorText);
            if (keystring == "")
            {
                Output.Text = Output.Text + "Errortext:" + errorText + "Done\r\n";
                return;
            }

            string tempfile = Path.GetTempPath() + "\\cert.html";
            StreamWriter fs = new StreamWriter(tempfile);
            fs.WriteLine("<html><head><META http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>TheSign Signature Certification</title></head><body><center>"
                + "<h2>OpenPGP (GPG) Signature Certification</h2>"
                + "<p>I herewith agree that my digital signature given and identified using OpenPGP (via TheSign/GPG) shall be valid and binding to the same extent as my handwrittten signature under any document.<p>"
                + "<p>Hiermit erkl&auml;re ich, dass meine digitale Unterschrift, die unter Verwendung von OpenPGP (mittels TheSign/GPG) abgegeben und identifiziert wird, in gleicher Weise g&uuml;ltig und bindend sein soll wie meine handschriftliche Unterschrift unter einem Dokument.<p>"
                + "<tt>"
                + outputText.Replace("\n", "<br>")
                + "</tt><p><p>Date:_______________________________________________"
                + "<p><p>Name_______________________________________________"
                + "<p><p>Signature_______________________________________________"
                + "<p><p>In case of data loss, here's the key as paper dump<p>"
                + "<p><p>F&uuml;r den Fall des Datenverlustes hier der Schl&uuml;ssel als Papier- Hardcopy<p>"
                + "<tt>"
                + keystring.Replace("\n", "<br>")
                + "</tt>"
                + "</body></html>");
            fs.Close();
            System.Diagnostics.Process.Start(tempfile);
            Output.Text = Output.Text + "\r\nDone\r\n";


        }

        private void launchURL(string targetURL)
        {
            System.Diagnostics.Process.Start(targetURL);
        }

        private void onlineManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/wiki/doku.php?id=thesign:index");
        }

        private void cleanupSigfile(string fileName)
        {
            Hashtable keys = new Hashtable();
            bool insidesign = false;
            string thisline;
            string totalsign = "";
            StreamReader fs = new StreamReader(fileName);
            while (!fs.EndOfStream)
            {
                thisline = fs.ReadLine();
                if (thisline == "-----BEGIN PGP SIGNATURE-----")
                {
                    totalsign = thisline;
                    insidesign = true;
                }
                else if (thisline == "-----END PGP SIGNATURE-----")
                {
                    if (insidesign)
                    {
                        insidesign = false;
                        totalsign += "\r\n" + thisline;
                        keys[totalsign] = 1;
                    }

                }
                else
                {
                    if (insidesign)
                    {
                        totalsign += "\r\n" + thisline;
                    }
                }
            }
            fs.Close();
            if (File.Exists(fileName))
            {
                File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
            }
            StreamWriter fsout = new StreamWriter(fileName);

            foreach (string line in keys.Keys)
            {
                fsout.WriteLine(line);
            }
            fsout.Close();
            File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);

        }

        private void bugtrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/flyspray/index.php?project=3&switch=1&do=index");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutwindow.ShowDialog();
        }

        private void toolStripButtonSendFileandSig_Click(object sender, EventArgs e)
        {
            SendFileandSig();
        }

        private void toolStripButtonSendSignaturesOnly_Click(object sender, EventArgs e)
        {
            SendSignaturesOnly();
        }

        private void toolStripButtoncheckEmailVality_Click(object sender, EventArgs e)
        {
            checkEmailValidity();
        }

        private void keyDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\pubring.gpg", "", Path.GetDirectoryName(Application.ExecutablePath) + "\\GnuPG\\pubring.gpg", true, false, true);
                if (filename != "")
                {
                    filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\secring.gpg", "", Path.GetDirectoryName(Application.ExecutablePath) + "\\GnuPG\\secring.gpg", true, false, true);
                }
                if (filename != "")
                {
                    MessageBox.Show("Key backup successful", "TheSign Key Backup");
                }
                else
                {
                    MessageBox.Show("Key backup failed", "TheSign Key Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void filesSignaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = (string)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "BrowseDir", Path.GetDirectoryName(Application.ExecutablePath));
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "BrowseDir", folderBrowserDialog.SelectedPath);
                foreach (string sourcefile in Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath) + "\\SignedFiles"))
                {
                    bool loop = true;
                    while (loop)
                    {
                        string filename = "";
                        if (Path.GetExtension(sourcefile) == ".sig")
                        {
                            filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + Path.GetFileName(sourcefile), "", sourcefile, true, false, true);
                            if (filename != "")
                            {
                                cleanupSigfile(filename);
                            }
                        }
                        else
                        {
                            filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + Path.GetFileName(sourcefile), "", sourcefile, true, false, false);
                        }
                        if (filename != "")
                        {
                            loop = false;
                        }
                        else
                        {
                            switch (MessageBox.Show("Couldn't copy " + Path.GetFileName(sourcefile), "TheSign FileCopy", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                            {
                                case DialogResult.Abort:
                                    goto endofForEach;
                                case DialogResult.Ignore:
                                    loop = false;
                                    break;
                                case DialogResult.Retry:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            endofForEach:
                { }
                MessageBox.Show("Data backup successful", "TheSign Data Backup");
            }
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
//            KeyRingData publicKey = new KeyForm.KeyRingData(false, gpg);
            KeyRingData publicKey = new KeyRingData(false, gpg);
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


        private void Sendkey()
        {
            if (keyview.SelectedNode != null)
            {
                string outputText = "";
                string errorText = "";
                gpg.command = Commands.ShowKey;
                gpg.armor = true;
                gpg.passphrase = "";
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
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
                    KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)node.Tag;
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
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)row.Tag;
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
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
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
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
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
                    MessageBox.Show(errorText, "GPG replies:");

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

        private void toolStripButton_SetTrustlevel_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem mybutton = (ToolStripItem)sender;
                int newtrustlevel = Convert.ToInt32(mybutton.Tag);
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
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

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            DeleteKey();
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
                            signature += thissign.Username + "; ";
                        }
                        SignGridView.Rows[SignGridView.RowCount - 1].Cells[2].Value = signature;
                        if (signature != "")
                        {
                            string missdepartments = "";
                            if (checkAuthorities(ref missdepartments, signs, SignGridView.RowCount - 1))
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
            processBar.Value = 0;
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

                        if (lastDate != DateTime.FromBinary(0))
                        {
                            thissign.email = email.Value;
                            if (thissign.email == "")
                            {
                                thissign.email = "unknownUser";
                                thissign.Username = "unknownUser";
                            }
                            else
                            {
                                r = new Regex(@"(?<="").*(?=\s*\()", RegexOptions.IgnoreCase);
                                email = r.Match(line);
                                thissign.Username = email.Value;
                                r = new Regex(@"(?<=\().*(?=\))", RegexOptions.IgnoreCase);
                                email = r.Match(line);
                                thissign.userComment = email.Value;
                            }
                            thissign.date = lastDate;

                            signs.Add(thissign);
                            lastDate = DateTime.FromBinary(0);
                        }
                    }

                }
                if (line.ToLower().Contains("public key not found")) //unknown Key
                {
                    signdata thissign = new signdata();
                    thissign.email = "unknownUser";
                    thissign.Username = "unknownUser";
                    thissign.date = lastDate;

                    signs.Add(thissign);
                    lastDate = DateTime.FromBinary(0);

                }


                // gpg: Signature made 02/14/08 08:45:40 
                if (line.ToLower().Contains("gpg: signature made"))
                {
                    Regex r = new Regex(@"\d{2}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}", RegexOptions.IgnoreCase);
                    Match datestring = r.Match(line);
                    try
                    {
                        //lastDate = DateTime.ParseExact(datestring.Value, "MM/dd/yy hh:mm:ss", culture, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                        lastDate = DateTime.Parse(datestring.Value, culture);
                    }
                    catch
                    {
                        sendBugReport("Auslesen des Datums in den Signatures", line, datestring.Value);
                    }
                }
            }
            return (signdata[])signs.ToArray(typeof(signdata));
        }

        private bool checkAuthorities(ref string missings, signdata[] signatures, int actRowCount)
        {
            Hashtable foundDepartments = new Hashtable();
            Hashtable departmentDates = new Hashtable();
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
                            try
                            {
                                if ((DateTime)departmentDates[department] > thisemail.date)
                                {
                                    departmentDates[department] = thisemail.date;
                                }
                            }
                            catch
                            {
                                departmentDates[department] = thisemail.date;//that's the first entry
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
                    missings += department + " ; ";
                }
            }
            foreach (string department in departmentDates.Keys)
            {
                SignGridView.Rows[actRowCount].Cells[SignGridView.Columns[department].Index].Value = Convert.ToDateTime(departmentDates[department], System.Globalization.DateTimeFormatInfo.CurrentInfo);
            }
            return missings == "";
        }

        private void checkfolder(string mypath)
        {
            authDepartments.Clear();
            SignGridView.RowCount = 0;
            SignGridView.ColumnCount = 4;
            processBar.Value = 0;
            if (System.IO.File.Exists(mypath + "\\authorities.xml"))
            {
                try
                {
                    xDoc = new XmlDocument();
                    xDoc.Load(mypath + "\\authorities.xml");
                    XmlNodeList departments = xDoc.GetElementsByTagName("department");
                    foreach (XmlNode thisnode in departments)
                    {
                        try
                        {
                            string department = thisnode.Attributes.GetNamedItem("name").Value;
                            SignGridView.Columns.Add(department, department);
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
                    Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign", "BrowseDir", folderBrowserDialog.SelectedPath);

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

        private void FolderDialogButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                checkfolder(folderBrowserDialog.SelectedPath);
            }
        }

        private void GoButton_Click(object sender, EventArgs e)
        {

            switch (ExportComboBox.SelectedIndex)
            {
                case 0:
                    string clip = "";
                    foreach (DataGridViewColumn col in SignGridView.Columns)
                    {
                        clip += col.HeaderText + "\t";
                    }

                    clip += "\r\n";

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
