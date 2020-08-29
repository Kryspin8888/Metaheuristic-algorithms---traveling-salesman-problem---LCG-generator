using LCG_Generator;
using ant_colony;
using SATSP;
using PathGATSP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfejsStartowy
{
    public partial class InterfejsStartowy : Form
    {
        
        public InterfejsStartowy()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                Form1 generator = new Form1();
                generator.Show();
            }
            else if ((int)comboBox1.SelectedIndex == 1)
            {
                ant_colony_s_path mrowkowy = new ant_colony_s_path();
                mrowkowy.Show();
            }
            else if ((int)comboBox1.SelectedIndex == 2)
            {
                MainForm wyzarzanie = new MainForm();
                wyzarzanie.Show();
            }
            else if ((int)comboBox1.SelectedIndex == 3)
            {
                GeneticForm genetic = new GeneticForm();
                genetic.Show();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
