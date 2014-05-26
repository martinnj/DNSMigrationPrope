namespace DNSMigrationPrope
{
    partial class SummaryPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.tbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SummaryProgress
            // 
            this.pbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbar.Location = new System.Drawing.Point(0, 0);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(556, 23);
            this.pbar.TabIndex = 0;
            // 
            // SummaryText
            // 
            this.tbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbox.Location = new System.Drawing.Point(0, 23);
            this.tbox.Multiline = true;
            this.tbox.Name = "tbox";
            this.tbox.Size = new System.Drawing.Size(556, 365);
            this.tbox.TabIndex = 1;
            // 
            // SummaryPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbox);
            this.Controls.Add(this.pbar);
            this.Name = "SummaryPage";
            this.Size = new System.Drawing.Size(556, 388);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar pbar;
        public System.Windows.Forms.TextBox tbox;
    }
}
