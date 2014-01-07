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

        TechTree.TechTree tree;
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int seed;
            if (!chkManualSeed.Checked || !int.TryParse(txtSeed.Text, out seed))
            {
                seed = r.Next(int.MinValue, int.MaxValue);
                txtSeed.Text = seed.ToString();
            }

            tree = new TechTree.TechTree(seed);
            tree.SortLayout();
            pictureBox1.Image = TreeBitmapWriter.WriteImage(tree, pictureBox1.Width, pictureBox1.Height);
            btnSort.Enabled = true;
        }

        private void chkManualSeed_CheckedChanged(object sender, EventArgs e)
        {
            txtSeed.ReadOnly = !chkManualSeed.Checked;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            tree.CondenseLayout();
            pictureBox1.Image = TreeBitmapWriter.WriteImage(tree, pictureBox1.Width, pictureBox1.Height);
        }
    }
}
