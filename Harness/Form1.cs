using DelayRepay_BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Harness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnHarvestTrainJourneyInfo_Click(object sender, EventArgs e)
        {
            DRHelper helper = new DRHelper();
            helper.DoSomething(chkConnected.Checked);
            MessageBox.Show("All Done");
        }

        private void btnTokenise_Click(object sender, EventArgs e)
        {
            TokeniserView tv = new TokeniserView();
            tv.ShowDialog();
        }

        private void btnTestEMail_Click(object sender, EventArgs e)
        {
            DRHelper helper = new DRHelper();
            helper.TestEMail();
        }
    }
}
