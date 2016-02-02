using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;

/* Make escape the key to abort download:
 * ->Create a separate thread for downloading
 * ->Change ways of downloading to make a list first before doing mass downloading
 * 
 * Just testing1
 */

namespace ErogeGalleryMaker
{
    public partial class MainForm : Form
    {
        
        public string diret = null;
        public string currentAddress = Directory.GetCurrentDirectory();
        public bool chosenFile;
        public bool notDone;
        public bool abortDLFlag;
        public Thread downloadThread;
        public List<DownloadObject> mainDL_List;
        public List<DownloadNameAndAddress> generatedDL_List;

        public MainForm()
        {
            InitializeComponent();
            this.openFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            this.folderBrowserDialog1.SelectedPath = System.IO.Directory.GetCurrentDirectory();
            mainDL_List = new List<DownloadObject>();
        }

        private delegate void EventHandle();
        private void updateBar()
        {
            progressBar1.Value++;
            progressBarLabel.Text = "(Press Esc to cancel) Downloading... (" + progressBar1.Value + "/" + progressBar1.Maximum + ")";
            progressBarLabel.Refresh();
            progressBar1.Refresh();

            if (progressBar1.Value == progressBar1.Maximum)
            {
                //Create a message dialog
                progressBarLabel.Text = "Complete!";
                progressBar1.Visible = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        #region Self Defined Functions

        // Bonus error catching for Download Command
        private void DL(List<DownloadNameAndAddress> oDNAAList, Label lbl, ProgressBar pBar)
        {
            WebClient downloadClient = new WebClient();
            foreach (DownloadNameAndAddress oDNAA in oDNAAList)
            {
                try
                {
                    downloadClient.DownloadFile(oDNAA.address, oDNAA.filename);
                }
                catch { }
            }
            Invoke(new EventHandle(updateBar));
            
        }

        // Returns true if move is successful
        // Returns false otherwise
        // Bonus error catching
        private bool MOVE(string sourceDirName, string destDirName)
        {
            try
            {
                Directory.Move(sourceDirName, destDirName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void askDialog()
        {
            Form2 chooseFileFolder = new Form2();
            chooseFileFolder.ShowDialog();
        }

        #endregion Self Defined FUnctions

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        #region Event Handler Functions

        private void button1_Click(object sender, EventArgs e)
        {
            //Section for resetting all values
            abortDLFlag = false;
            mainDL_List.Clear();
            mainDL_List.TrimExcess();
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Visible = true;
            progressBarLabel.Visible = true;

            openFileDialog1.Title = "Select Input TXT file";
            openFileDialog1.Filter = "Text Files|*.txt";
            openFileDialog1.Multiselect = false;

            if (!(openFileDialog1.ShowDialog() == DialogResult.Cancel))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName, System.Text.Encoding.Default, true);

                string line = null;

                
                // Create a download list
                while (true)
                {
                    line = sr.ReadLine();
                    if (String.IsNullOrEmpty(line) || abortDLFlag)
                    {
                        sr.Close();
                        break;
                    }
                    mainDL_List.Add(new DownloadObject(line));
                }
                sr.Close();

                //Find number of games to download screenshots of
                progressBar1.Maximum = mainDL_List.Count;

                //The main download loop
                new Thread (() => massDownloader(mainDL_List)).Start();
            }
        }

        private void massDownloader(List<DownloadObject> mainDL_List)
        {
            foreach (DownloadObject oDO in mainDL_List)
            {
                generatedDL_List = oDO.generateDownloadList();
                downloadThread = new Thread(() => DL(generatedDL_List, progressBarLabel, progressBar1));
                downloadThread.Start();
                while (downloadThread.IsAlive) { Thread.Sleep(10); };
            }
        }
        
        //Find DLSite specific html location and read it
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select all HTML files";
            openFileDialog1.Filter = "HTML Files|*.html";
            openFileDialog1.Multiselect = true;

            if (!(openFileDialog1.ShowDialog() == DialogResult.Cancel))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileNames[0], System.Text.Encoding.Default, true);
                sr.Close();

                string line = null, title = null, circle = null, date = null, filename = null, fileDir = null;
                string[] delList = new String[100];
                int i = 0, j = 0, k = 0, index = 0, position = 0;

                for (index = 0; index < 100; index++)
                {
                    try
                    {
                        sr = new System.IO.StreamReader(openFileDialog1.FileNames[index]);
                        filename = openFileDialog1.SafeFileNames[index];
                        fileDir = openFileDialog1.FileNames[index].Replace(filename, "");
                        filename = filename.Substring(0, 8);

                        for (int temp = 0; temp < filename.Length - 2; temp++)
                        {
                            if (String.Equals(filename.Substring(temp, 2), "RJ") && char.IsDigit(filename[temp + 2]))
                            {
                                position = temp;
                                break;
                            }
                            else if (String.Equals(filename.Substring(temp, 2), "RE") && char.IsDigit(filename[temp + 2]))
                            {
                                position = temp;
                                break;
                            }
                            else if (String.Equals(filename.Substring(temp, 2), "VJ") && char.IsDigit(filename[temp + 2]))
                            {
                                position = temp;
                                break;
                            }

                        }

                        // Find the title line in HTML code
                        Boolean flagTitleFound = false;
                        while (!flagTitleFound)
                        {
                            line = sr.ReadLine();
                            for (int ind_tex = 0; ind_tex < line.Length - 7; ind_tex++)
                            {
                                if (String.Equals(line.Substring(ind_tex, 7), "<title>"))
                                {
                                    i = ind_tex + 7;
                                    flagTitleFound = true;
                                }
                                else if (line[ind_tex] == '[')
                                {
                                    j = ind_tex + 1;
                                }
                                else if (line[ind_tex] == ']')
                                {
                                    k = ind_tex;
                                    break;
                                }
                            }
                        }

                        title = line.Substring(i, j - i - 2);
                        circle = line.Substring(j, k - j);

                        // Search for this line <table cellspacing="0" id="work_outline"> then read the next line
                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            for (int ind_tex = 0; ind_tex < line.Length - 6; ind_tex++)
                            {
                                if (String.Equals(line.Substring(ind_tex, 6), "/year/"))
                                {
                                    for (int ind_tex2 = 0; ind_tex2 < line.Length - 4; ind_tex2++)
                                    {
                                        if (String.Equals(line.Substring(ind_tex2, 4), "year"))
                                        {
                                            i = ind_tex2 + 7;
                                        }
                                        else if (String.Equals(line.Substring(ind_tex2, 3), "mon"))
                                        {
                                            j = ind_tex2 + 4;
                                        }
                                        else if (String.Equals(line.Substring(ind_tex2, 3), "day"))
                                        {
                                            k = ind_tex2 + 4;
                                            break;
                                        }
                                    }
                                    date = line.Substring(i, 2) + line.Substring(j, 2) + line.Substring(k, 2);
                                    break;
                                }
                            }
                        }

                        MOVE(fileDir + filename + "_img_main.jpg", fileDir + filename + "\\" + filename + "_img_main.jpg");
                        MOVE(fileDir + filename + "_img_smp1.jpg", fileDir + filename + "\\" + filename + "_img_smp1.jpg");
                        MOVE(fileDir + filename + "_img_smp2.jpg", fileDir + filename + "\\" + filename + "_img_smp2.jpg");
                        MOVE(fileDir + filename + "_img_smp3.jpg", fileDir + filename + "\\" + filename + "_img_smp3.jpg");
                        MOVE(fileDir + filename + "_img_smp4.jpg", fileDir + filename + "\\" + filename + "_img_smp4.jpg");
                        MOVE(fileDir + filename + "_img_smp5.jpg", fileDir + filename + "\\" + filename + "_img_smp5.jpg");
                        MOVE(fileDir + filename + "_img_smp6.jpg", fileDir + filename + "\\" + filename + "_img_smp6.jpg");
                        MOVE(fileDir + filename + "_img_smp7.jpg", fileDir + filename + "\\" + filename + "_img_smp7.jpg");
                        MOVE(fileDir + filename + "_img_smp8.jpg", fileDir + filename + "\\" + filename + "_img_smp8.jpg");
                        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                        foreach (char c in invalid)
                        {
                            circle = circle.Replace(c.ToString(), " ");
                            title = title.Replace(c.ToString(), " ");
                        }

                        sr.Close();
                        if (MOVE(fileDir + filename, fileDir + "[" + circle + "] [" + date + "] " + title))
                        {
                            System.IO.File.Delete(fileDir + filename + ".html");
                        }
                    }
                    catch
                    {
                        sr.Close();
                        sr.Dispose();
                        // break;
                    }
                }
                MessageBox.Show("Move and renaming RJ folder is complete.");
            }
        }

        #endregion Event Handler Functions

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                abortDLFlag = true;
            }
        }

        private void progressBarLabel_TextChanged(object sender, EventArgs e)
        {
            //center the text
            progressBarLabel.Location = new Point(this.Width / 2 - progressBarLabel.Width / 2, progressBarLabel.Location.Y);
        }

    }
}
