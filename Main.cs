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

        //the object to wrap the GPG access
        GnuPGWrapper gpg = new GnuPGWrapper();
        KeyRingData.KeyRingUser ownerKeyId = null;

        // the window to input the PassPhrase
        PassphraseForm testDialog = new PassphraseForm();
        // the About Window
        AboutBox aboutwindow = new AboutBox();
        // the last checked file
        string lastcheckedFile = "";

        /// <summary>
        /// the class with stores the public key datas
        /// </summary>
        public class KeyRingData
        {
            /// <summary>
            /// The class which stores the data about one public key entry
            /// </summary>
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
            // List of all key contained in that keyring
            public ArrayList keyEntries = new ArrayList();
            // Culture to provide the american date format used by the GPG output
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", false);
            /// <summary>
            /// Constructor which reads the keyring from the given GPG object
            /// </summary>
            /// <param name="seckeyring">read the public (false) or the secret (true) keyring</param>
            /// <param name="gpg">the GPGwrapper object</param>
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
                // splits the GPG output in it's lines
                foreach (string line in (outputText.Split('\n')))
                {
                    // and split the line into its values
                    string[] thisline = line.Split(':');
                    try
                    {
                        //is it a key description?
                        if (thisline[0] == "sec" || thisline[0] == "pub")
                        {
                            // did we just filled an entry in the previous loop?
                            // then save the old entry first
                            if (thisKeyringUser != null)
                            {
                                keyEntries.Add(thisKeyringUser);
                            }

                            thisKeyringUser = new KeyRingUser();
                            thisKeyringUser.UserId = thisline[4];
                            thisKeyringUser.User = thisline[9];
                            // filter the users email address out of the user data
                            Regex r = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}", RegexOptions.IgnoreCase);
                            Match regExMatch = r.Match(thisKeyringUser.User);
                            thisKeyringUser.userEmail = regExMatch.Value;
                            // filter the users name out of the user data
                            r = new Regex(@".*(?=(\s*<))", RegexOptions.IgnoreCase);
                            regExMatch = r.Match(thisKeyringUser.User);
                            thisKeyringUser.Username = regExMatch.Value;
                            // filter the users comment out of the user data
                            r = new Regex(@"(?<=\().*(?=\))", RegexOptions.IgnoreCase);
                            regExMatch = r.Match(thisKeyringUser.User);
                            thisKeyringUser.userComment = regExMatch.Value;
                            // get the key generation date
                            thisKeyringUser.Generated = DateTime.Parse(thisline[5], culture);
                            // is the key expired?
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
                        // store the keys fingerprint
                        if (thisline[0] == "fpr")
                        {
                            thisKeyringUser.Fingerprint = thisline[9];
                        }
                        // the the signatures for that key
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
                    keyEntries.Add(thisKeyringUser);
                }


            }
        }

        private Hashtable trustLevels = new Hashtable();
        XmlDocument xDoc;
        Hashtable authDepartments = new Hashtable();
        IFormatProvider culture = new System.Globalization.CultureInfo("en-US", false);

        /// <summary>
        /// collected the details of a file signature
        /// </summary>
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
            // load last used directory where to store files
            openFileDialog.InitialDirectory = (string)Registry.GetValue(tsConst.RegKey, tsConst.RegFileDir, Path.GetDirectoryName(Application.ExecutablePath));
            // saves the path of this TheSign instance as info for the update installer
            Registry.SetValue(tsConst.RegKey, tsConst.RegExePath, Path.GetDirectoryName(Application.ExecutablePath));
            gpg.homedirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir;
            gpg.passphrase = "signtest";
            // creates the keylist
            buildTree();
            // loads the secret key list to determine the actual name of the theSign user
            KeyRingData privateKey = new KeyRingData(true, gpg);
            int i = 0;
            bool endflag = false;
            // running through the list of known secret keys
            while (i < privateKey.keyEntries.Count && !endflag)
            {
                ownerKeyId = (KeyRingData.KeyRingUser)privateKey.keyEntries[i];
                // looking for the first valid entry
                if (ownerKeyId.valid)
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
                // valid secret key found
                // set originator in the GPGP object
                gpg.originator = ownerKeyId.UserId;
                // and set the window title
                Text += " | " + ownerKeyId.User;
                FileInfo tempFile = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.revKeyFile);
                if (!tempFile.Exists)
                {
                    if (MessageBox.Show("You do not have a revokation file to revoke your signature file if necessary\nDo you want to create it now?", "Generate Key revokation?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Revokekey();
                    }

                }
            }
            else
            {
                Text = "Installation incomplete" + " | " + Text + " | " + build.version + "  " + build.buildver;
                MessageBox.Show("No valid Userid found - Installation incomplete?");
            }
            // load last browsed folder from the registry
            folderBrowserDialog.SelectedPath = (string)Registry.GetValue(tsConst.RegKey, tsConst.RegBrowseDir, Path.GetDirectoryName(Application.ExecutablePath));
            // check if Browsefolder contains a authority file
            checkForAuthorityFile(folderBrowserDialog.SelectedPath);
            // preselect the browse list output format
            ExportComboBox.SelectedIndex = 0;

        }

        /// <summary>
        /// Destructor writes the last used file folder into registry
        /// </summary>
        ~Main()
        {
            Registry.SetValue(tsConst.RegKey, tsConst.RegFileDir, openFileDialog.InitialDirectory);

        }

        /// <summary>
        /// Handles dropped files or dropped email attachments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Output_DragDrop(object sender, DragEventArgs e)
        {

            string[] fileNames = null;

            // bring own window to front
            this.Activate();
            try
            {
                // Is it a normal file from the local filesystem dropped into the window?
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
                // or is it a dropped email- attachment?
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
                    // put thefile into the temp directory
                    string path = Path.GetTempPath();
                    // create the full-path name
                    string theFile = path + fileName.ToString();

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
                        Output.Text = Output.Text + "\r\nError: " + "File was not created!";
                    }
                }
            }
            catch (Exception ex)
            {
                Output.Text = Output.Text + "\r\nError: " + ex.Message;
                // don't use MessageBox here - Outlook or Explorer is waiting !
            }
        }


        /// <summary>
        /// Puts the last checked file and signature into an email
        /// </summary>
        private void SendFileandSig()
        {
            // did we already checked a file?
            if (lastcheckedFile != "")
            {
                // was it a signature?
                if (Path.GetExtension(lastcheckedFile).ToLower() == tsConst.sigExt)
                {
                    // remove the signature extension
                    lastcheckedFile = Path.GetDirectoryName(lastcheckedFile) + "\\" + Path.GetFileNameWithoutExtension(lastcheckedFile);
                }
                //and put file and signature in an email
                Outlook.MailItem actMail = MakeNewMail();
                if (actMail != null)
                {
                    actMail.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    actMail.Subject = "File & Sig of  " + Path.GetFileName(lastcheckedFile) + " [TheSign]";
                    actMail.HTMLBody = "<html><body><p><i>Please add a few friendly words to the receipient here :-)</i></p><span style=\"font-size:0.6em\">(made by <a href=\"http://www.koehlers.de/wiki/doku.php?id=thesign:index\">TheSign</a>)</span>";
                    try
                    {
                        actMail.Attachments.Add(lastcheckedFile, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Attachments.Add(lastcheckedFile + tsConst.sigExt, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Display(true);
                    }
                    catch
                    {
                        MessageBox.Show("Error: Can't attach file\n" + lastcheckedFile, "TheSign Error");
                    }
                }
            }
        }

        /// <summary>
        /// For better debugging, puts an error discription and a set of preselected 
        /// variables into an email to the program author
        /// </summary>
        /// <param name="title"></param>
        /// <param name="variables"></param>
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
                errorlist = errorlist + "var " + i.ToString() + ":" + myobj.ToString() + "\r\n";
                i++;
            }
            Outlook.MailItem actMail = MakeNewMail();
            if (actMail != null)
            {
                actMail.To = tsConst.progAuthor;
                actMail.Subject = "TheSign Error Report";
                actMail.Body = errorlist;
                actMail.Display(true);

            }
        }

        /// <summary>
        /// Signs and sends the last checked file text output
        /// this odd function is used when somebody needs to be informed about
        /// a signature, but if he's not allowed to see the signed document itself
        /// </summary>
        private void SendSignaturesOnly()
        {
            string text = Output.Text;
            if (text != "")
            {

                string outputText = "";
                string errorText = "";
                if (testDialog.ShowDialog("Sign last output text?") == DialogResult.OK)
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
                        Outlook.MailItem actMail = MakeNewMail();
                        if (actMail != null)
                        {
                            actMail.Subject = "TheSign: Confirmation of Signatures";
                            actMail.Body = "This is a mail generated by the TheSign (http://www.koehlers.de/wiki/doku.php?id=thesign:index)\n\nHere's the confirmation that " + Path.GetFileName(lastcheckedFile) + " is signed as follows\n" + outputText;
                            actMail.Display(true);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// if a signed text is received via Email, this function checks if the text
        /// signature is valid
        /// </summary>
        private void checkEmailValidity()
        {
            // get body text from actual selected email
            Outlook.MailItem actMail = GetActiveMail();
            if (actMail != null)
            {
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

        }

        /// <summary>
        /// this functions does the global handling of all incoming file requests
        /// and decides based on the file extension and the isMail flag
        /// how to handle that file
        /// </summary>
        /// <param name="fileName">the complete path for the file</param>
        /// <param name="isMail">true is file is dropped from an email</param>
        private void HandleFile(string fileName, Boolean isMail)
        {
            // is it a signature?
            if (Path.GetExtension(fileName).ToLower() == tsConst.sigExt)
            {
                if (isMail)
                {
                    // if the signature comes from an email, locate the original 
                    // document first
                    openFileDialog.Title = "Select original file to add the sign to:";
                    openFileDialog.FileName = Path.GetFullPath(openFileDialog.InitialDirectory) + "\\" + Path.GetFileName((Path.GetFileNameWithoutExtension(fileName)));
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        openFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                        // save the actual folder in the registry for the next time..
                        Registry.SetValue(tsConst.RegKey, tsConst.RegFileDir, openFileDialog.InitialDirectory);
                        string targetFileName = openFileDialog.FileName;
                        // remove the signature extension
                        while (Path.GetExtension(targetFileName).ToLower() == tsConst.sigExt)
                        {
                            targetFileName = Path.GetFileNameWithoutExtension(targetFileName);
                        }
                        lastcheckedFile = targetFileName;
                        // append the signature to an exisiting signature files
                        targetFileName = MoveIntoFile(targetFileName, tsConst.sigExt, fileName, true, true, true,true);
                        Output.Text += "\r\nSignature saved";
                        // remove duplicates in the signature file
                        cleanupSigfile(targetFileName);
                        // and show the actual signatures
                        CheckSig(targetFileName, false);
                        // and finally switch some buttons on
                        toolStripButtonSendFileandSig.Enabled = true;
                        toolStripButtonSendSignaturesOnly.Enabled = true;
                    }
                }
                else
                {
                    // if signature is a normal file, just check it
                    CheckSig(fileName, true);
                    lastcheckedFile = fileName;
                    toolStripButtonSendFileandSig.Enabled = true;
                    toolStripButtonSendSignaturesOnly.Enabled = true;
                }
            }
            else if (Path.GetExtension(fileName).ToLower() == tsConst.pubKeyExt)
            {
                // import the key file
                importKey(fileName);
                lastcheckedFile = "";
                toolStripButtonSendFileandSig.Enabled = false;
                toolStripButtonSendSignaturesOnly.Enabled = false;

            }
            else
            {
                if (isMail)
                {
                    // save the file first
                    fileName = MoveIntoFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\SignedFiles\\" + Path.GetFileName(fileName), "", fileName, true, false, false,true);
                    if (fileName != "")
                    {
                        // Sign it
                        SignFile(fileName);
                        // and send the signature back
                        ReplyMail(" [TheSign]", "<html><body><p><i>Please add a few friendly words to the receipient here :-)</i></p><span style=\"font-size:0.6em\">(made by <a href=\"http://www.koehlers.de/wiki/doku.php?id=thesign:index\">TheSign</a>)</span>", fileName + tsConst.sigExt);
                        lastcheckedFile = fileName;
                        toolStripButtonSendFileandSig.Enabled = true;
                        toolStripButtonSendSignaturesOnly.Enabled = true;

                    }
                }
                else
                {
                    // just sign it
                    SignFile(fileName);
                    lastcheckedFile = fileName;
                    toolStripButtonSendFileandSig.Enabled = true;
                    toolStripButtonSendSignaturesOnly.Enabled = true;

                }
                if (fileName != "")
                {
                    // and remember the directory
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                    Registry.SetValue(tsConst.RegKey, tsConst.RegFileDir, openFileDialog.InitialDirectory);
                }
            }
        }

        /// <summary>
        /// compares two files if equal or not
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns>true if equal</returns>
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

        /// <summary>
        /// Overwrites or appends another file or text string to a file
        /// </summary>
        /// <param name="fileName">the output file</param>
        /// <param name="ext">if given, ext is added to filename as extension</param>
        /// <param name="input">text string or filename as input</param>
        /// <param name="isFile">if true, input is handled as filename</param>
        /// <param name="Append">if true, input will append to the output file, 
        /// otherways the output file will be overwritten</param>
        /// <param name="overwrite">if true, overwrite output without confirmation window</param>
        /// <returns></returns>
        private string MoveIntoFile(string fileName, string ext, string input, bool isFile, bool Append, bool overwrite, bool setReadOnly)
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
                // open the input
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
                    // open the output for append or rewrite
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
                // copy input to output
                while ((bytesread = inputStream.Read(buffer, 0, 1024)) > 0)
                {
                    fs.Write(buffer, 0, bytesread);
                }
                inputStream.Close();
                fs.Close();
                // and set the file to readonly again
                if (setReadOnly) File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);

            }
            else
            {
                // in case a text string should be written to the output file
                if (File.Exists(fileName))
                {
                    // Remove read only first
                    File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
                }
                StreamWriter fs = new StreamWriter(fileName, Append);
                fs.Write(input);
                fs.Close();
                if (setReadOnly) File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);
            }
            return fileName;
        }

        /// <summary>
        /// replies to actual Outlook Mail by quoting the old body text and the subject
        /// </summary>
        /// <param name="subject">the mail Subject</param>
        /// <param name="body">the Mail body</param>
        /// <param name="attachmentName">the attachment to add</param>
        /// <returns></returns>
        private int ReplyMail(String subject, String body, String attachmentName)
        {
            Outlook.MailItem ReplyMail = MakeReplyMail();
            if (ReplyMail != null)
            {
                try
                {
                    ReplyMail.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    ReplyMail.Subject = ReplyMail.Subject + subject;
                    ReplyMail.HTMLBody = body + ReplyMail.HTMLBody;
                    if (attachmentName != "")
                    {
                        ReplyMail.Attachments.Add(attachmentName, Type.Missing, Type.Missing, Type.Missing);
                    }
                    ReplyMail.Display(true);
                }
                catch
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Import public keys from a file
        /// </summary>
        /// <param name="fileName"></param>
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
            buildTree();
        }

        /// <summary>
        /// checks the signatures of a file
        /// </summary>
        /// <param name="fileName">the file to check</param>
        /// <param name="clearfirst">clear output window first if true</param>
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
            gpg.ExecuteCommand("", fileName, out outputText, out errorText);
            gpg.batch = true;
            Output.Text = Output.Text + Path.GetFileName(fileName) + " checked:\r\n" + errorText;
        }

        /// <summary>
        /// signs a file
        /// </summary>
        /// <param name="fileName">the file to sign</param>
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
                    //Attach output signature text to signature file
                    MoveIntoFile(fileName, tsConst.sigExt, outputText, false, true, false,true);
                    File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);
                    Output.Text = Output.Text + "\r\nSuccess: " + Path.GetFileName(fileName) + " signed\r\n";
                    CheckSig(fileName + tsConst.sigExt, false);
                }
                else
                {
                    MessageBox.Show(errorText, "GPG replies:");

                }


            }
            else
            {
                Output.Text = Output.Text + "\r\nCanceled";

            }
        }

        /// <summary>
        /// generated a key revoke file
        /// </summary>
        private void Revokekey()
        {
            clearWindow();
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.RevokeKey;
            gpg.armor = true;
            gpg.batch = false;
            Output.Text = Output.Text + "\r\nGenerate key revoke file ...";
            gpg.ExecuteCommand("", Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.revKeyFile, out outputText, out errorText);
            gpg.batch = true;
            Output.Text = Output.Text + "\r\n" + outputText;
        }

        /// <summary>
        /// Handles the incoming DragEnter- Events, if a file or a email
        /// attachment is dropped into the program window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Quits the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Quit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Cleans up the window outputs to handle a new input file
        /// </summary>
        private void clearWindow()
        {
            Output.Text = "";
        }

        /// <summary>
        /// Generates a html output of the users fingerprint and present it
        /// in a Browser window for printing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                + "<p>This agreement is valid until I declare my digital signature as not longer valid by distribute my revoke key.<p>"
                + "<p>Hiermit erkl&auml;re ich, dass meine digitale Unterschrift, die unter Verwendung von OpenPGP (mittels TheSign/GPG) abgegeben und identifiziert wird, in gleicher Weise g&uuml;ltig und bindend sein soll wie meine handschriftliche Unterschrift unter einem Dokument.<p>"
                + "<p>Diese Einverst&auml;ndnis gilt bis zu dem Zeitpunkt, an dem ich meine digitale Unterschrift durch Verteilen meines Revoke Keys f&uuml;r nicht weiter g&uuml;tig erkl&auml;re.<p>"
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

        /// <summary>
        /// Load a given URL in a Browser window
        /// </summary>
        /// <param name="targetURL">the URL to show</param>
        private void launchURL(string targetURL)
        {
            System.Diagnostics.Process.Start(targetURL);
        }

        /// <summary>
        /// Load the online manual in a browser window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onlineManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/wiki/doku.php?id=thesign:index");
        }

        /// <summary>
        /// removes duplicate entries out of a signature file
        /// </summary>
        /// <param name="fileName">the signature file to clean up</param>
        private void cleanupSigfile(string fileName)
        {
            // little Trick: using a hash table to eleminate duplicates
            // by using the signature as key, so double signatures will
            // end up as a single hash
            Hashtable keys = new Hashtable();
            bool insidesign = false;
            string thisline;
            string totalsign = "";
            StreamReader fs = new StreamReader(fileName);
            // read through the file line by line
            while (!fs.EndOfStream)
            {
                thisline = fs.ReadLine();
                // concat all lines of a signature into one single string
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
                        //and finally using the signature string as key for a hash,
                        // filled with a dummy value
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
            // close the read input
            fs.Close();
            // remove the write protection
            if (File.Exists(fileName))
            {
                File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
            }
            StreamWriter fsout = new StreamWriter(fileName);
            // and write the keys of the previously generated hashtable (=the signatures)
            // back into the file
            foreach (string line in keys.Keys)
            {
                fsout.WriteLine(line);
            }
            fsout.Close();
            // finally set the write protection again
            File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.ReadOnly);

        }

        /// <summary>
        /// loads the TheSign Bugtracker in a browser window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bugtrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/flyspray/index.php?project=3&switch=1&do=index");
        }

        /// <summary>
        /// Opens the About- Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutwindow.ShowDialog();
        }

        /// <summary>
        /// put the last checked file and its signatures into an email to sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSendFileandSig_Click(object sender, EventArgs e)
        {
            SendFileandSig();
        }

        /// <summary>
        /// signs the gpg output of the last checked file and put it in an email to
        /// send as confirmation that a particular file has been signed
        /// (needed if the recipient needs the confirmation about a file signature,
        /// but is not permitted to see the file itself)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSendSignaturesOnly_Click(object sender, EventArgs e)
        {
            SendSignaturesOnly();
        }

        /// <summary>
        /// Checks the signature in an received email text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtoncheckEmailVality_Click(object sender, EventArgs e)
        {
            checkEmailValidity();
        }

        /// <summary>
        /// Creates a backup of the public and secret key rings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + tsConst.pubKeyFile, "", Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.pubKeyFile, true, false, true, true);
                if (filename != "")
                {
                    filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + tsConst.secKeyFile, "", Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.secKeyFile, true, false, true,true);
                }
                if (filename != "")
                {
                    filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + tsConst.revKeyFile, "", Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.revKeyFile, true, false, true, true);
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

        /// <summary>
        /// Creates Backups of the locally existing files and signatures into a
        /// selectable folder.
        /// While doing that, files are checked for equally and signature files are merged
        /// together
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filesSignaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = (string)Registry.GetValue(tsConst.RegKey, tsConst.RegBrowseDir, Path.GetDirectoryName(Application.ExecutablePath));
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Registry.SetValue(tsConst.RegKey, tsConst.RegBrowseDir, folderBrowserDialog.SelectedPath);
                foreach (string sourcefile in Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath) + "\\SignedFiles"))
                {
                    bool loop = true;
                    while (loop)
                    {
                        string filename = "";
                        if (Path.GetExtension(sourcefile) == tsConst.sigExt)
                        {
                            filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + Path.GetFileName(sourcefile), "", sourcefile, true, false, true,true);
                            if (filename != "")
                            {
                                cleanupSigfile(filename);
                            }
                        }
                        else
                        {
                            filename = MoveIntoFile(folderBrowserDialog.SelectedPath + "\\" + Path.GetFileName(sourcefile), "", sourcefile, true, false, false,true);
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

        /// <summary>
        /// Load actual trustlevel DB into Trustlevels-Array
        /// </summary>
        /// <returns>true if successful</returns>
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
                        trustLevels[thisline[0]] = Convert.ToInt32(thisline[1], 10);
                    }
                    catch
                    { }
                }
                i++;
            }
            return trustLevels.Count > 0;
        }

        /// <summary>
        /// Saves the actual Trustlevels-Array into the trustlevel DB 
        /// </summary>
        /// <returns>tru if successful</returns>
        private bool SaveDB()
        {
            if (trustLevels.Count > 0)
            {
                string outputText = "";
                string errorText = "";
                string keytext = "";
                foreach (string key in trustLevels.Keys)
                {
                    keytext += key + ":" + trustLevels[key].ToString() + "\n";
                }
                gpg.command = Commands.writeTrust;
                gpg.armor = true;
                gpg.passphrase = "";
                gpg.ExecuteCommand(keytext, "", out outputText, out errorText);

            }
            return true;
        }

        /// <summary>
        /// Lookup the actual keys trust level
        /// </summary>
        /// <param name="fingerprint">the fingerprint of the requested user</param>
        /// <returns>the keys trustlevel</returns>
        private int getTrustLevel(string fingerprint)
        {
            int trustlevel = 0;
            try
            {
                trustlevel = (int)trustLevels[fingerprint] - 2;
            }
            catch
            {
            }
            return trustlevel;
        }

        /// <summary>
        /// Loads the public keys and creates the nodeview list of the keys
        /// </summary>
        private void buildTree()
        {
            LoadDB();
            //            KeyRingData publicKey = new KeyForm.KeyRingData(false, gpg);
            KeyRingData publicKey = new KeyRingData(false, gpg);
            keyview.Nodes.Clear();
            foreach (KeyRingData.KeyRingUser user in publicKey.keyEntries)
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

        /// <summary>
        /// Opens a window to select which key should be sended to who and put the 
        /// pubkey file than into an email with the previous selected receiptients
        /// </summary>
        private void Sendkey()
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
                gpg.ExecuteCommandMultiple("", keyIDs, out outputText, out errorText, true);
                if (outputText != "")
                {
                    Outlook.MailItem actMail = MakeNewMail();
                    if (actMail != null)
                    {
                        actMail.Subject = "TheSign: Some Public Keys of other users";
                        actMail.Body = "The attached .pubkey file contains the public key(s) of the following users:\n\n";
                        actMail.Body = actMail.Body + users;
                        actMail.Body = actMail.Body + "\n\nTo add or update these users in your public key ring, just drag and drop the pubkey attachment into your TheSign Window\n\n";
                        //                        actMail.Body = actMail.Body + outputText;
                        actMail.To = emails;
                        string tempfile = Path.GetTempPath() + "\\theSign.pubkey";
                        StreamWriter fs = new StreamWriter(tempfile);
                        fs.WriteLine(outputText);
                        fs.Close();
                        actMail.Attachments.Add(tempfile, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Display(true);
                        Activate();
                        File.Delete(tempfile);
                    }
                }
            }
            else
            {
                MessageBox.Show("You need to select at least one \"Key\" to send something :-)", "TheSign Hint");
            }
        }

        /// <summary>
        /// Signs a key in the keyring
        /// </summary>
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

        /// <summary>
        /// Deletes a key in the keyring
        /// </summary>
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

        /// <summary>
        /// switches the toolbar on and off, depenting if a key is slected or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// modifies the trust level of the selected key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_SetTrustlevel_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem mybutton = (ToolStripItem)sender;
                int newtrustlevel = Convert.ToInt32(mybutton.Tag);
                KeyRingData.KeyRingUser myuser = (KeyRingData.KeyRingUser)keyview.SelectedNode.Tag;
                trustLevels[myuser.Fingerprint] = newtrustlevel + 1;
                SaveDB();
                buildTree();
            }
        }

        /// <summary>
        /// send some keys via email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSend_Click(object sender, EventArgs e)
        {
            Sendkey();
        }

        /// <summary>
        /// Signs a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripSign_Click(object sender, EventArgs e)
        {
            SignKey();
        }

        /// <summary>
        /// deletes a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            DeleteKey();
        }

        /// <summary>
        /// starts a scan run for valid signatures in the BrowseDirectory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (Path.GetExtension(fileName).ToLower() != tsConst.sigExt)
                {
                    SignGridView.Rows.Add();
                    SignGridView.Rows[SignGridView.RowCount - 1].Cells[0].Value = Path.GetFileName(fileName);
                    if (System.IO.File.Exists(fileName + tsConst.sigExt))
                    {
                        signdata[] signs = new signdata[0];
                        string outputText = "";
                        string errorText = "";
                        gpg.command = Commands.Verify;
                        gpg.armor = true;
                        gpg.batch = false; //Somehow the --batch flag makes GPG to not return all signs, just some...
                        gpg.passphrase = "";
                        gpg.ExecuteCommand("", fileName + tsConst.sigExt, out outputText, out errorText);
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

        /// <summary>
        /// transfers the GPG signature check output into a signData
        /// structure (like Username, date etc.)
        /// </summary>
        /// <param name="text"></param>
        /// <returns>a filled out signData object</returns>
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

        /// <summary>
        /// checks, if a set of given signDatas fulfill the nesserary
        /// signature rules given in the authories file of the browsed directory
        /// </summary>
        /// <param name="missings">list of departments which signature is missing</param>
        /// <param name="signatures">Array of signdata for the actual file</param>
        /// <param name="actRowCount">reference to the row in the gridview where to store the results</param>
        /// <returns></returns>
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
                        if (thisnode.InnerText.ToLower() == thisemail.email.ToLower())
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

        /// <summary>
        /// check if given folder contains a valid authority file
        /// and enables the start button accourdingly
        /// </summary>
        /// <param name="mypath">folder path</param>
        private void checkForAuthorityFile(string mypath)
        {
            authDepartments.Clear();
            SignGridView.RowCount = 0;
            SignGridView.ColumnCount = 4;
            processBar.Value = 0;
            if (System.IO.File.Exists(mypath + "\\" + tsConst.authFile))
            {
                try
                {
                    xDoc = new XmlDocument();
                    xDoc.Load(mypath + "\\" + tsConst.authFile);
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
                    Registry.SetValue(tsConst.RegKey, tsConst.RegBrowseDir, folderBrowserDialog.SelectedPath);

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

        /// <summary>
        /// Opens the folderDialog and reads in the authority file, if the folder
        /// contains a vaild authority file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderDialogButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                checkForAuthorityFile(folderBrowserDialog.SelectedPath);
            }
        }

        /// <summary>
        /// Exports the actual signature table of the SignBrowser, actual only as CSV data
        /// into the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Connects to Outlook
        /// </summary>
        /// <returns></returns>
        private Outlook._Application OpenOutlook()
        {
            try
            {
                // See details in http://support.microsoft.com/kb/302902/
                return new Outlook.Application();
            }
            catch
            {
                MessageBox.Show("Error: No connection to Outlook found..", "TheSign Error");
                return null;
            }

        }


        /// <summary>
        /// Creates a new emtpy Outlook mail
        /// </summary>
        /// <returns></returns>
        private Outlook.MailItem MakeNewMail()
        {
            try
            {
                // connect to Outlook
                Outlook._Application outLookApp = OpenOutlook();

                Outlook.MailItem actMail = (Outlook.MailItem)outLookApp.CreateItem(Outlook.OlItemType.olMailItem);
                return actMail;
            }
            catch
            {
                MessageBox.Show("Error: Email could not been created..", "TheSign Error");
                return null;
            }
        }

        /// <summary>
        /// get the actual mail from Outlook
        /// </summary>
        /// <returns></returns>
        private Outlook.MailItem GetActiveMail()
        {
            try
            {
                Outlook._Application outLookApp = OpenOutlook();
                // search for the active element
                Outlook._Explorer myExplorer = outLookApp.ActiveExplorer();
                // is one item selected?
                if (myExplorer.Selection.Count == 1)
                {
                    //is it a Mail?
                    if (myExplorer.Selection[1] is Outlook.MailItem)
                    {
                        // then reply it
                        return (Outlook.MailItem)myExplorer.Selection[1];
                    }
                    else
                    {
                        MessageBox.Show("Sorry, the Item in Outlook seems no mail..", "TheSign Error");
                        return null;
                    }

                }
                else
                {
                    MessageBox.Show("Sorry, there's more as one item selected in Outlook", "TheSign Error");
                    return null;
                }

            }
            catch
            {
                MessageBox.Show("Error: Email could not been found..", "TheSign Error");
                return null;
            }
        }

        /// <summary>
        /// Creates a Mail object as a reply to the actual one
        /// </summary>
        /// <returns></returns>
        private Outlook.MailItem MakeReplyMail()
        {
            Outlook.MailItem actMail = GetActiveMail();
            if (actMail != null)
            {
                try
                {
                    return actMail.Reply();
                }
                catch
                {
                    MessageBox.Show("Error: Could not create Reply Mail..", "TheSign Error");
                    return null;
                }

            }
            return null;
        }

        /// <summary>
        /// Distributes the revokation key to all other known users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteOwnKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This funktion makes your key unusable\nDo you REALLY want this?", "Own Key Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                if (MessageBox.Show("Again: Do you REALLY want to DELETE your own key?", "Emergency Warning!!", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    KeyRingData publicKey = new KeyRingData(false, gpg);
                    string emails = "";
                    foreach (KeyRingData.KeyRingUser user in publicKey.keyEntries)
                    {
                        emails += user.userEmail + "; ";
                    }
                    Outlook.MailItem actMail = MakeNewMail();
                    if (actMail != null)
                    {
                        actMail.Subject = "TheSign: Revoke key of " + ownerKeyId.User;
                        actMail.Body = "The attached .pubkey file contains the revoke key from " + ownerKeyId.User + ", who wants to declare his actual key as invalid\n\n";
                        actMail.Body = actMail.Body + "\n\nTo add this revokation information to your public key ring, just drag and drop the pubkey attachment into your TheSign Window\n\n";
                        //                        actMail.Body = actMail.Body + outputText;
                        actMail.To = emails;
                        string tempfile = Path.GetTempPath() + "\\revoke.pubkey";
                        MoveIntoFile(tempfile, "", Path.GetDirectoryName(Application.ExecutablePath) + "\\" + tsConst.gpgDir + "\\" + tsConst.revKeyFile, true, false, true,false);
                        actMail.Attachments.Add(tempfile, Type.Missing, Type.Missing, Type.Missing);
                        actMail.Display(true);
                        Activate();
                        File.Delete(tempfile);
                    }


                }
            }
        }

    }



    /// <summary>
    /// Generic class to store the important program constants
    /// </summary>
    static class tsConst
    {
        // The extension of the signature files
        public const string sigExt = ".sig";
        // The extension of the public key files
        public const string pubKeyExt = ".pubkey";
        // Where in the registry the data is stored
        public const string RegKey = "HKEY_CURRENT_USER\\SOFTWARE\\Koehler_Programms\\TheSign";
        // the key for the last browsed directory
        public const string RegBrowseDir = "BrowseDir";
        // the key for the install directory of the sign
        // need by the installer to installing updates
        public const string RegExePath = "ExePath";
        //the key of where to store received files
        public const string RegFileDir = "FileDir";
        // the name of the authorisation file
        public const string authFile = "authorities.xml";
        // the name of the public key file
        public const string pubKeyFile = "pubring.gpg";
        // the name of the secret key file
        public const string secKeyFile = "secring.gpg";
        // the name of the revoke key file
        public const string revKeyFile = "revokkey.gpg";
        // the GPG subdirectory
        public const string gpgDir = "GnuPG";
        // the program author (for debug emails
        public const string progAuthor = "steffen@koehlers.de";
    }
}
