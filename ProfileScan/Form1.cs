using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Threading;

namespace ProfileScan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                //ThreadPool.QueueUserWorkItem(filesearch);
                FilesearchWorker.RunWorkerAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int numdirs;

        //delegate void AddListItemDelegate(string name);

        //private void updatelistbox(string foldname)
        //{
        //    try
        //    {
        //        listBox1.Items.Add(foldname);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //private void filesearch(object state)
        //{
        //    try
        //    {
        //        toolStripStatusLabel1.Text = ("Getting Directory: " + textBox1.Text);
        //        DirectoryInfo di = new DirectoryInfo(textBox1.Text);

        //        foreach (var fold in di.GetDirectories("*.v2"))
        //        {
        //            toolStripStatusLabel1.Text = ("Getting Directory: " + fold.Name);
        //            foreach (var fi in fold.GetFiles("NTUSER.DAT"))
        //            {
        //                if (fi.Length <= 262144)
        //                {
        //                    BeginInvoke(new AddListItemDelegate(updatelistbox), fold.Name);
        //                }
        //            }
        //        }
        //        toolStripStatusLabel1.Text = ("Done");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(("Error occurred: " + ex.Message));
        //        toolStripStatusLabel1.Text = ("Error: " + ex.Message);
        //        throw;
        //    }
        //}

        private void FilesearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = ("Getting Directory: " + textBox1.Text);
                DirectoryInfo di = new DirectoryInfo(textBox1.Text);
                numdirs = di.GetDirectories("*.v2").Length;
                int i = 0;
                double percentdone = ((double)i / numdirs) * 100.0;

                foreach (var fold in di.GetDirectories("*.v2"))
                {
                    if (FilesearchWorker.CancellationPending == true)
                    {
                        toolStripStatusLabel1.Text = "Cancelled!";
                        break;
                    }

                    i++;
                    percentdone = ((double)i / numdirs) * 100.0;

                    toolStripStatusLabel1.Text = ("Getting Directory " + i + ": " + fold.Name );
                    foreach (var fi in fold.GetFiles("NTUSER.DAT"))
                    {
                        if (FilesearchWorker.CancellationPending == true)
                        {
                            toolStripStatusLabel1.Text = "Cancelled!";
                            break;
                        }

                        if (fi.Length <= 262144)
                        {
                            //BeginInvoke(new AddListItemDelegate(updatelistbox), fold.Name);
                            FilesearchWorker.ReportProgress((int)percentdone, fold.Name);
                        }
                        else
                        {
                            //MessageBox.Show(("Progress: 100/" + numdirs + "*" + i + "=" + (100 / numdirs) * i));
                            FilesearchWorker.ReportProgress((int)percentdone, null);

                        }
                    }
                }
                toolStripStatusLabel1.Text = ("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(("Error occurred: " + ex.Message));
                toolStripStatusLabel1.Text = ("Error: " + ex.Message);
                throw;
            }

        }

        private void FilesearchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState != null)
                {
                    listBox1.Items.Add(e.UserState);
                }
                //progressBar1.Maximum = numdirs;
                progressBar1.Value = e.ProgressPercentage;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FilesearchWorker.CancelAsync();
        }
    }

}

