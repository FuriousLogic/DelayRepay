namespace Harness
{
    partial class TokeniserView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tvWebPage = new System.Windows.Forms.TreeView();
            this.btnGetFile = new System.Windows.Forms.Button();
            this.lblFilename = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvWebPage
            // 
            this.tvWebPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvWebPage.Location = new System.Drawing.Point(12, 68);
            this.tvWebPage.Name = "tvWebPage";
            this.tvWebPage.Size = new System.Drawing.Size(507, 381);
            this.tvWebPage.TabIndex = 0;
            // 
            // btnGetFile
            // 
            this.btnGetFile.Location = new System.Drawing.Point(12, 13);
            this.btnGetFile.Name = "btnGetFile";
            this.btnGetFile.Size = new System.Drawing.Size(25, 23);
            this.btnGetFile.TabIndex = 1;
            this.btnGetFile.Text = "...";
            this.btnGetFile.UseVisualStyleBackColor = true;
            this.btnGetFile.Click += new System.EventHandler(this.btnGetFile_Click);
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(44, 22);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(0, 13);
            this.lblFilename.TabIndex = 2;
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(12, 42);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(426, 20);
            this.txtURL.TabIndex = 3;
            this.txtURL.Text = "http://realtime.nationalrail.co.uk/cct/sumarr.aspx?T=NMP";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(444, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TokeniserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 461);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.btnGetFile);
            this.Controls.Add(this.tvWebPage);
            this.Name = "TokeniserView";
            this.Text = "TokeniserView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvWebPage;
        private System.Windows.Forms.Button btnGetFile;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Button button1;
    }
}