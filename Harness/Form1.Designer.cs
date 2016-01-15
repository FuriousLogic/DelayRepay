namespace Harness
{
    partial class Form1
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
            this.btnHarvestTrainJourneyInfo = new System.Windows.Forms.Button();
            this.chkConnected = new System.Windows.Forms.CheckBox();
            this.btnTokenise = new System.Windows.Forms.Button();
            this.btnTestEMail = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHarvestTrainJourneyInfo
            // 
            this.btnHarvestTrainJourneyInfo.Location = new System.Drawing.Point(13, 13);
            this.btnHarvestTrainJourneyInfo.Name = "btnHarvestTrainJourneyInfo";
            this.btnHarvestTrainJourneyInfo.Size = new System.Drawing.Size(151, 23);
            this.btnHarvestTrainJourneyInfo.TabIndex = 0;
            this.btnHarvestTrainJourneyInfo.Text = "HarvestTrainJourneyInfo";
            this.btnHarvestTrainJourneyInfo.UseVisualStyleBackColor = true;
            this.btnHarvestTrainJourneyInfo.Click += new System.EventHandler(this.btnHarvestTrainJourneyInfo_Click);
            // 
            // chkConnected
            // 
            this.chkConnected.AutoSize = true;
            this.chkConnected.Location = new System.Drawing.Point(182, 17);
            this.chkConnected.Name = "chkConnected";
            this.chkConnected.Size = new System.Drawing.Size(78, 17);
            this.chkConnected.TabIndex = 1;
            this.chkConnected.Text = "Connected";
            this.chkConnected.UseVisualStyleBackColor = true;
            // 
            // btnTokenise
            // 
            this.btnTokenise.Location = new System.Drawing.Point(13, 42);
            this.btnTokenise.Name = "btnTokenise";
            this.btnTokenise.Size = new System.Drawing.Size(151, 23);
            this.btnTokenise.TabIndex = 2;
            this.btnTokenise.Text = "Tokenise Web Page";
            this.btnTokenise.UseVisualStyleBackColor = true;
            this.btnTokenise.Click += new System.EventHandler(this.btnTokenise_Click);
            // 
            // btnTestEMail
            // 
            this.btnTestEMail.Location = new System.Drawing.Point(13, 71);
            this.btnTestEMail.Name = "btnTestEMail";
            this.btnTestEMail.Size = new System.Drawing.Size(151, 23);
            this.btnTestEMail.TabIndex = 3;
            this.btnTestEMail.Text = "Test EMail";
            this.btnTestEMail.UseVisualStyleBackColor = true;
            this.btnTestEMail.Click += new System.EventHandler(this.btnTestEMail_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.btnTestEMail);
            this.Controls.Add(this.btnTokenise);
            this.Controls.Add(this.chkConnected);
            this.Controls.Add(this.btnHarvestTrainJourneyInfo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnHarvestTrainJourneyInfo;
        private System.Windows.Forms.CheckBox chkConnected;
        private System.Windows.Forms.Button btnTokenise;
        private System.Windows.Forms.Button btnTestEMail;
    }
}

