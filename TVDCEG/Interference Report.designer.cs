namespace TVDCEG
{
    partial class Interference_Report
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interference_Report));
            this.Show = new System.Windows.Forms.Button();
            this.Export = new System.Windows.Forms.Button();
            this.listBoxshow = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Refesh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Show
            // 
            this.Show.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Show.Location = new System.Drawing.Point(105, 403);
            this.Show.Name = "Show";
            this.Show.Size = new System.Drawing.Size(86, 25);
            this.Show.TabIndex = 1;
            this.Show.Text = "Show";
            this.Show.UseVisualStyleBackColor = true;
            this.Show.Click += new System.EventHandler(this.Show_Click);
            // 
            // Export
            // 
            this.Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Export.Location = new System.Drawing.Point(198, 403);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(86, 25);
            this.Export.TabIndex = 2;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // listBoxshow
            // 
            this.listBoxshow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxshow.FormattingEnabled = true;
            this.listBoxshow.Location = new System.Drawing.Point(12, 20);
            this.listBoxshow.Name = "listBoxshow";
            this.listBoxshow.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxshow.Size = new System.Drawing.Size(437, 368);
            this.listBoxshow.TabIndex = 3;
            this.listBoxshow.SelectedIndexChanged += new System.EventHandler(this.listBoxshow_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(363, 401);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Refesh
            // 
            this.btn_Refesh.Location = new System.Drawing.Point(12, 403);
            this.btn_Refesh.Name = "btn_Refesh";
            this.btn_Refesh.Size = new System.Drawing.Size(86, 25);
            this.btn_Refesh.TabIndex = 9;
            this.btn_Refesh.Text = "Refesh";
            this.btn_Refesh.UseVisualStyleBackColor = true;
            this.btn_Refesh.Click += new System.EventHandler(this.btn_Refesh_Click);
            // 
            // Interference_Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 438);
            this.Controls.Add(this.btn_Refesh);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBoxshow);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.Show);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Interference_Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Interference Report";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Interference_Report_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private new System.Windows.Forms.Button Show;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.ListBox listBoxshow;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_Refesh;
    }
}