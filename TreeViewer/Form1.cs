using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TreeViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int seed;
            if (!chkManualSeed.Checked || !int.TryParse(txtSeed.Text, out seed))
            {
                seed = r.Next(int.MinValue, int.MaxValue);
                txtSeed.Text = seed.ToString();
            }

            TechTree.TechTree tree = new TechTree.TechTree(seed);
            pictureBox1.Image = TreeBitmapWriter.WriteImage(tree, pictureBox1.Width, pictureBox1.Height);
        }

        private void chkManualSeed_CheckedChanged(object sender, EventArgs e)
        {
            txtSeed.ReadOnly = !chkManualSeed.Checked;
        }
    }
}
