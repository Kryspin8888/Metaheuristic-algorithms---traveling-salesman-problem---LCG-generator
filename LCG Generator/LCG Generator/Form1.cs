using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LCG_Generator
{
    public partial class Form1 : Form
    {
        private System.Threading.ManualResetEvent _busy = new System.Threading.ManualResetEvent(false);
        long i;
        Bitmap DrawArea;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.BackColor = Color.White;
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
                i = 0;
            }

            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < pictureBox1.Height / 8; i++)
            {
                g.DrawLine(System.Drawing.Pens.Black, 0, i * 8, pictureBox1.Right, i * 8);
                g.DrawLine(System.Drawing.Pens.Black, i * 8, 0, i * 8, pictureBox1.Bottom);
            }
            //g.FillRectangle(System.Drawing.Brushes.Chocolate, 0, 0, 20, 20);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Graphics g = Graphics.FromImage(DrawArea);

            long x0, x, a, c, m;
            x0 = int.Parse(seedTextBox.Text);
            a = int.Parse(aTextBox.Text);
            c = int.Parse(cTextBox.Text);
            m = int.Parse(mTextBox.Text);
            x = x0;

            do
            {
                x = (a * x + c) % m;
                g.FillRectangle(System.Drawing.Brushes.Chocolate, 
                    (((x % 3844) - ((x % 3844)/62)*62))*8, (x % 3844)/62*8, 8, 8);
                pictureBox1.Image = DrawArea;

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(30);
                    worker.ReportProgress(10);
                }
                i++;
            } while (x != x0);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)

        {
            label2.Text = (i.ToString());
        }

        private void label1_Click(object sender, EventArgs e)
             
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            backgroundWorker1.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
