using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SATSP
{
    public partial class MainForm : Form
    {
        private float[] x;
        private float[] y;

        private double alpha;
        private double T0;

        private int N;
        private int m;
        private int n;
        private int seed;

        private int count1;
        private int count2;
        private int count3;
        private int count4;

        private int[] A0;
        private int[] a0;
        private int[] aF;

        private double fF;

        private BackgroundWorker bw;
        private DateTime oldDate, newDate;

        public MainForm()
        {
            InitializeComponent();
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) +
                Math.Pow(y1 - y2, 2.0));
        }

        private double TourDistance(int[] city)
        {
            double td = 0.0;
            int number = city.Length;

            for (int i = 0; i < number - 1; i++)
                td += Distance(x[city[i]], y[city[i]],
                    x[city[i + 1]], y[city[i + 1]]);

            td += Distance(x[city[number - 1]], y[city[number - 1]],
                x[city[0]], y[city[0]]);

            return td;
        }

        private void Initialize(int[] city, Random random)
        {
            int c, num = 0, number = city.Length;
            bool[] used = new bool[number];
            
            for (int i = 0; i < number; i++)
                used[i] = false;

            do
            {
                c = random.Next(number);

                if (used[c])
                    continue;

                used[c] = true;
                city[num] = c;
                num++;
            } while (num < number);
        }

        private void SimulatedAnnealing()
        {
            // algorithm by Professor and Chair Alice E. Smith
            // alpha = exponential annealing schedule parameter in (0, 1)
            // standard = standard deviation for randomGaussian function
            // T0 = initial temperature
            // N = number of facilities and sites
            // m = number of temperatures
            // n = number of moves at a given temperature
            // aF = final value of assignment vector
            // A0 = initial value of assignment vector
            // fF = final value of function f
            // count1 = number of descending moves
            // count2 = number of Metropolis accepted uphill moves
            // count3 = number of rejected Metropolis moves
            // count4 = number of changes in final function values
            // count1 + count2 + count3 = m * n

            double T, f0, f1;
            int ai, u, v, mn = m * n;

            Random random = new Random(seed);

            a0 = new int[N];
            aF = new int[N];
            A0 = new int[N];

            // select random starting point
            // (random permutation of 0 to N - 1)

            Initialize(A0, random);

            for (int i = 0; i < A0.Length; i++)
            {
                ai = A0[i];
                a0[i] = ai;
                aF[i] = ai;
            }

            f0 = f1 = fF = TourDistance(A0);
            T = T0 = mn;

            count1 = count2 = count3 = count4 = 0;

            if (fF == 0)
                return;

            for (int t = 1; t <= m; t++)
            {
                for (int i = 1; i <= n; i++)
                {
                    if (bw.CancellationPending)
                        return;

                    int[] aTemp = new int[N];

                    if (random.NextDouble() < 0.4)
                    {
                        // calculate move (swap)

                        do
                        {
                            u = random.Next(N);
                            v = random.Next(N);
                        } while (u == v);

                        for (int j = 0; j < N; j++)
                            aTemp[j] = a0[j];

                        int temp = aTemp[u];

                        aTemp[u] = aTemp[v];
                        aTemp[v] = temp;
                    }

                    else
                    {
                        // inversion

                        int cp1 = -1, cp2 = -1;

                        do
                        {
                            cp1 = random.Next(N);
                            cp2 = random.Next(N);
                        } while (cp1 == cp2 || cp1 > cp2);

                        int k = cp1;
                        int l = cp2;

                        for (int j = 0; j < N; j++)
                            aTemp[j] = a0[j];

                        for (int q = cp1; q <= cp2; q++)
                            aTemp[k++] = a0[l--];
                    }

                    // calculate f1

                    f1 = TourDistance(aTemp);

                    if (f1 <= f0)
                    {

                        // update a0 and f0

                        for (int j = 0; j < N; j++)
                            a0[j] = aTemp[j];

                        f0 = f1;
                        count1++;
                    }
                    else
                    {
                        // Metropolis part of algorithm

                        if (random.NextDouble() <= Math.Exp(-(f1 - f0) / T))
                        {
                            // accept uphill move
                            // update a0 and f0

                            for (int j = 0; j < N; j++)
                                a0[j] = aTemp[j];

                            f0 = f1;
                            count2++;
                        }
                        else
                        {
                            // reject uphill move
                            count3++;
                        }
                    }

                    if (f1 <= fF)
                    {

                        // update final assignment vector and final f

                        for (int j = 0; j < N; j++)
                            aF[j] = aTemp[j];

                        fF = f1;
                        count4++;

                        if (fF == 0)
                            return;
                    }

                    // exponential annealing schedule

                    T = alpha * T;
                }

                bw.ReportProgress((int)(100.0 * t) / m);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(
                    openFileDialog1.FileName);

                string line = sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    if (line.Contains("DIMENSION"))
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < line.Length; i++)
                        {
                            char c = line[i];

                            if (c >= '0' && c <= '9')
                                sb.Append(c);
                        }

                        N = int.Parse(sb.ToString());
                        
                        break;
                    }

                    line = sr.ReadLine();
                }

                x = new float[N];
                y = new float[N];
                
                int count = 0;

                line = sr.ReadLine();

                while (line != null && line.Length != 0)
                {
                    if (line[0] >= '0' && line[0] <= '9')
                    {
                        StringBuilder sb = new StringBuilder();
                        int i, j, k = 0;

                        while (line[k] == ' ')
                            k++;

                        for (i = k; i < line.Length; i++)
                        {
                            char c = line[i];

                            if (c >= '0' && c <= '9')
                                sb.Append(c);

                            else
                                break;
                        }

                        k = i;

                        while (line[k] == ' ')
                            k++;

                        sb = new StringBuilder();

                        for (j = k; j < line.Length; j++)
                        {
                            char c = line[j];

                            if (c >= '0' && c <= '9' || c == '.')
                                sb.Append(c);

                            else
                                break;
                        }

                        x[count] = float.Parse(sb.ToString());

                        k = j;

                        while (line[k] == ' ')
                            k++;

                        sb = new StringBuilder();

                        for (i = k; i < line.Length; i++)
                        {
                            char c = line[i];

                            if (c >= '0' && c <= '9' || c == '.')
                                sb.Append(c);

                            else
                                break;
                        }

                        y[count++] = float.Parse(sb.ToString());
                    }
                 
                    line = sr.ReadLine();
                }

                sr.Close();
                button1.Enabled = false;
                button2.Enabled = true;
                textBox4.Text = N.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Start")
            {
                oldDate = DateTime.Now;
                alpha = double.Parse(textBox1.Text);
                m = int.Parse(textBox2.Text);
                n = int.Parse(textBox3.Text);
                seed = int.Parse(textBox5.Text);
                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync();
                while (!bw.IsBusy) { }
                button2.Text = "S&top";
            }
            else
                bw.CancelAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SimulatedAnnealing();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox6.Text = null;

            for (int i = 0; i < N; i++)
                textBox6.Text += i.ToString() + "\t" +
                    aF[i].ToString() + "\r\n";

            textBox6.Text += N.ToString() + "\t" +
                    aF[0].ToString() + "\r\n";

            textBox6.Text += "\r\nTour Length = " +
                fF.ToString("F6") + "\r\n";

            newDate = DateTime.Now;
            TimeSpan ts = newDate - oldDate;

            textBox6.Text += "\r\nRuntime (Hrs:Min:Sec.MS) = ";
            textBox6.Text += ts.Hours.ToString("00") + ":";
            textBox6.Text += ts.Minutes.ToString("00") + ":";
            textBox6.Text += ts.Seconds.ToString("00") + ".";
            textBox6.Text += ts.Milliseconds.ToString("000") + "\r\n";
            button2.Text = "&Solve";
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grafika grafika = new Grafika(x,y,aF,N);
            grafika.Show();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
