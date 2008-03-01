using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emmanuel.Cryptography.GnuPG;
using System.Diagnostics;
using System.IO;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections;


namespace TheSign
{
    public partial class Main : Form
    {
        
          GnuPGWrapper gpg = new GnuPGWrapper();
          PassphraseForm testDialog = new PassphraseForm();
          KeyForm keywindow;
      public Main()
      {
          InitializeComponent();
          saveFileDialog.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath);
          gpg.homedirectory = Path.GetDirectoryName( Application.ExecutablePath)+"\\GnuPG";
          gpg.passphrase = "signtest";
          keywindow = new KeyForm(gpg, testDialog);
          KeyForm.KeyRingData privateKey = new KeyForm.KeyRingData(true, gpg);

          int i=0;
          bool endflag=false;
          KeyForm.KeyRingData.KeyRingUser thisitem = null;
          while (i < privateKey.Items.Count && !endflag)
          {
              thisitem = (KeyForm.KeyRingData.KeyRingUser)privateKey.Items[i];
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
        }

        private void Output_DragDrop(object sender, DragEventArgs e)
        {

            string[] fileNames = null;

           

                try
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                    {
                        fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                        // handle each file passed as needed
                        foreach (string fileName in fileNames)
                        {
                            HandleFile(fileName,false);
                        }
                    }
                    else if (e.Data.GetDataPresent("FileGroupDescriptor"))
                    {
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

                            //tempFile.Delete();
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

        private void HandleFile(string fileName, Boolean isMail)
        {
            // do what you are going to do with each filename
            if (Path.GetExtension(fileName) == ".sig")
            {
                if (isMail)
                {
                    saveFileDialog.Title = "Select original file to add the sign to:";
                    saveFileDialog.FileName = Path.GetFullPath(saveFileDialog.InitialDirectory) + "\\" + Path.GetFileName((Path.GetFileNameWithoutExtension(fileName)));
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = MoveIntoFile(saveFileDialog.FileName, ".sig", fileName, true, true, true);
                        Output.Text += "\r\nSignature saved";
                        cleanupSigfile(fileName);
                        CheckSig(fileName);
                    }
                }
                else
                {
                    CheckSig(fileName);

                }
            }
            else if (Path.GetExtension(fileName) == ".pubkey")
            {
                importKey(fileName);
            }
            else
            {
                if (isMail)
                {
                    fileName = MoveIntoFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\SignedFiles\\" + Path.GetFileName(fileName), ".sig", fileName, true, true, false);
                    if (fileName!="")
                    {
                        SignFile(fileName);
                        ReplyMail("TheSign: Signature of " + Path.GetFileName(fileName), "This is an automatic generated message\n\n Attached you'll find the gpg-signature of the file " + Path.GetFileName(fileName), fileName + ".sig");
                    }
                }
                else
                {
                    SignFile(fileName);
                }
            }
        }

        private string MoveIntoFile(string fileName, string ext, string input, bool isFile, bool Append, bool overwrite)
        {
            fileName += ext;
            if (isFile)
            {
                if (File.Exists(fileName) && !overwrite)
                {
                    if (MessageBox.Show("The file " + fileName + " already exits.\nDo you really want to overwite?","Ok to overwrite?", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    {
                        return "";
                    }
                }
                int bytesread;
                byte[] buffer = new byte[1024];
                Stream inputStream = File.OpenRead(input);
                Stream fs;
                if (Append)
                {
                    fs = File.Open(fileName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fs = File.OpenWrite(fileName);
                }

                while ((bytesread = inputStream.Read(buffer, 0, 1024)) > 0)
                {
                    fs.Write(buffer, 0, bytesread);
                }
                inputStream.Close();
                fs.Close();
            }
            else
            {
                StreamWriter fs = new StreamWriter(fileName , Append);
                fs.Write(input);
                fs.Close();
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
                        ReplyMail.Subject = subject;
                        ReplyMail.Body = body;
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
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Import;
            gpg.armor = true;
            Output.Text = Output.Text + "\r\nImport " + Path.GetFileName(fileName) + " ...";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", fileName, out outputText, out errorText);
            Output.Text = Output.Text + Path.GetFileName(fileName) + " Imported:\r\n" + errorText;
        }

        private void CheckSig(String fileName)
        {
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Verify;
            gpg.armor = true;
            Output.Text = Output.Text + "\r\nChecking " + Path.GetFileName(fileName) + " ...";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", fileName, out outputText, out errorText);
            Output.Text = Output.Text + Path.GetFileName(fileName) + " checked:\r\n" + errorText;
        }

        private void SignFile(String fileName)
        {
            string outputText = "";
            string errorText = "";
            if (testDialog.ShowDialog() == DialogResult.OK)
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
                    MoveIntoFile(fileName, ".sig", outputText, false, true,false);
                    Output.Text = Output.Text + "\r\nSuccess: " + Path.GetFileName(fileName) + " signed " + outputText;
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

        private void Menu_listKeys(object sender, EventArgs e)
        {
            keywindow.ShowDialog();
        }

        private void Menu_printCertificate(object sender, EventArgs e)
        {
            string outputText = "";
            string errorText = "";
            gpg.command = Commands.Fingerprint;
            gpg.armor = true;
            Output.Text = Output.Text + "\r\nGenerate Certificate sheet:\r\n";
            gpg.passphrase = "";
            gpg.ExecuteCommand("", "", out outputText, out errorText);
            if (outputText == "")
            {
                Output.Text = Output.Text + "Errortext:"+errorText+"Done\r\n";
            }
            else
            {
                string tempfile = Path.GetTempPath()+"\\cert.html";
                StreamWriter fs = new StreamWriter(tempfile);
                fs.WriteLine("<html><head><META http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>TheSign Signature Certification</title></head><body><center>"
                    +"<h2>GPG Signature Certification</h2>"
                    +"<p>Hereby I declare that my dignal signature shown below"
                    +"can be used equally to my normal handwritten signature until redingsbums<p>"
                    +"<p>Hiermit erkläre ich, das meine unten aufgeführte digitale Signatur"
                    +"gleichbedeutend meiner normalen handschriftlichen Unterschrift betrachtet werden kann<p>"
                    +"<tt>"
                    +outputText.Replace("\n","<br>")
                    +"</tt><p><p>Date:_______________________________________________"
                    + "<p><p>Name_______________________________________________"
                    + "<p><p>Signature_______________________________________________"
                    + "</body></html>");
                fs.Close();
                System.Diagnostics.Process.Start(tempfile);
                Output.Text = Output.Text  + "\r\nDone\r\n";
            }

        }

        private void launchURL(string targetURL)
        {
            System.Diagnostics.Process.Start(targetURL);
        }

        private void onlineManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/wiki/doku.php?id=thesign:index");
        }

        private void cleanupSigfile(string filename)
        {
            Hashtable keys =new Hashtable();
            bool insidesign = false;
            string thisline;
            string totalsign="";
            StreamReader fs = new StreamReader(filename);
            while (!fs.EndOfStream)
            {
                thisline= fs.ReadLine();
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
            StreamWriter fsout = new StreamWriter(filename);
            
            foreach (string line in keys.Keys)
            {
                fsout.WriteLine(line);
            }
            fsout.Close();

        }

        private void bugtrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchURL("http://www.koehlers.de/flyspray/index.php?project=3&switch=1&do=index");
        }
    }
}
