using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SATSP
{
    public partial class Grafika : Form
    {
        Bitmap DrawArea;
        float[] xx;
        float[] yy;
        int[] aFF;
        int NN;
        public Grafika(float []x,float []y, int []aF, int N)
        {
            InitializeComponent();
            xx = x;
            yy = y;
            aFF = aF;
            NN = N;
            pictureBox1.BackColor = Color.White;
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;

        }

        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < NN; i++)
            {              
                g.FillEllipse(Brushes.Chocolate, xx[i] %pictureBox1.Width, yy[i]%pictureBox1.Height, 10, 10);
                g.DrawLine(System.Drawing.Pens.Black, xx[i] % pictureBox1.Width+5, yy[i] % pictureBox1.Height+5, xx[aFF[i]] % pictureBox1.Width+5, yy[aFF[i]] % pictureBox1.Height+5);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
