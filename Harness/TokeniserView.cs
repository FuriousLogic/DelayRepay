using DelayRepay_BL.Tokenise;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Harness
{
    public partial class TokeniserView : Form
    {
        public TokeniserView()
        {
            InitializeComponent();
        }

        private void btnGetFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName == "") return;

            lblFilename.Text = ofd.FileName;

            populateTreeview();
        }

        private void populateTreeview()
        {
            Tokeniser tokeniser = new Tokeniser();

            string html = "";
            if (txtURL.Text == "")
            {
                FileInfo fi = new FileInfo(lblFilename.Text);
                StreamReader sr = fi.OpenText();
                html = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                Uri site = new Uri(txtURL.Text);
                WebRequest wReq = WebRequest.Create(site);
                WebResponse wResp = wReq.GetResponse();
                if (wResp != null)
                {
                    Stream respStream = wResp.GetResponseStream();
                    if (respStream != null)
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.ASCII);
                        html = reader.ReadToEnd();
                    }
                }
            }

            List<WebItem> webItems = tokeniser.Tokenise(html);
            if (webItems.Count != 1)
            {
                MessageBox.Show("No single root to webitem");
                return;
            }

            //WebItem detailRows = Tokeniser.ExtractSection(webItems[0], "table", "title", "Previous Calling Points");
            WebItem detailRows = Tokeniser.ExtractSection(webItems[0], "table", "id", "TrainTable");
            detailRows = Tokeniser.ExtractSection(detailRows, "tbody", "", "");

            //3924208
            showWebitem(null, detailRows);
        }

        private void showWebitem(TreeNode parentNode, WebItem webItem)
        {
            TreeNode childNode = new TreeNode(webItem.Text);
            if (parentNode == null)
            {
                tvWebPage.Nodes.Clear();
                tvWebPage.Nodes.Add(childNode);
            }
            else
                parentNode.Nodes.Add(childNode);

            //Attributes
            foreach (WebItemAttribute wia in webItem.Attributes)
            {
                TreeNode ndAtt = new TreeNode("(a)" + wia.Name + " = " + wia.Content);
                childNode.Nodes.Add(ndAtt);
            }

            //Children
            foreach (WebItem childWI in webItem.Children)
            {
                showWebitem(childNode, childWI);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            populateTreeview();
        }
    }
}
