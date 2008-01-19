using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emmanuel.Cryptography.GnuPG;

using System.Diagnostics;
using System.IO;

using System.Collections;


namespace TheSign
{
    public partial class Main : Form
    {
        GnuPGWrapper gpg = new GnuPGWrapper();
        PassphraseForm testDialog = new PassphraseForm();
        public Main()
        {
            InitializeComponent();
            gpg.command = Commands.detachsign;

            // Set some parameters from on Web.Config file
            gpg.homedirectory = "./GnuPG";
            gpg.passphrase = "signtest";
        }

        private void Output_DragDrop(object sender, DragEventArgs e)
        {

            string[] fileNames = null;
            string outputText = "";

            if (testDialog.ShowDialog() == DialogResult.OK)
            {
                gpg.passphrase = testDialog.passPhraseText.Text;
                if (!testDialog.storePassPhrase.Checked)
                {
                    testDialog.passPhraseText.Text = "";
                }


                try
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                    {
                        fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                        // handle each file passed as needed
                        foreach (string fileName in fileNames)
                        {
                            // do what you are going to do with each filename
                            Output.Text = Output.Text + "\r\nSigning " + Path.GetFileName(fileName) + " ...";
                            gpg.ExecuteCommand(fileName, out outputText);
                            Output.Text = Output.Text + "\r\nSuccess: " + Path.GetFileName(fileName) + " signed " + outputText;

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

        private void Output_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
