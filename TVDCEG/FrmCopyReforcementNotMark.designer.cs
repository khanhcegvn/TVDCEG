namespace TVDCEG
{
    partial class FrmCopyReforcementNotMark
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCopyReforcementNotMark));
            this.lbx_Showelement = new System.Windows.Forms.ListBox();
            this.ckb_Reforcement = new System.Windows.Forms.CheckBox();
            this.ckb_Connection = new System.Windows.Forms.CheckBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbx_Showelement
            // 
            this.lbx_Showelement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbx_Showelement.FormattingEnabled = true;
            this.lbx_Showelement.Location = new System.Drawing.Point(12, 12);
            this.lbx_Showelement.Name = "lbx_Showelement";
            this.lbx_Showelement.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbx_Showelement.Size = new System.Drawing.Size(348, 303);
            this.lbx_Showelement.TabIndex = 0;
            // 
            // ckb_Reforcement
            // 
            this.ckb_Reforcement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckb_Reforcement.AutoSize = true;
            this.ckb_Reforcement.Location = new System.Drawing.Point(12, 340);
            this.ckb_Reforcement.Name = "ckb_Reforcement";
            this.ckb_Reforcement.Size = new System.Drawing.Size(87, 17);
            this.ckb_Reforcement.TabIndex = 1;
            this.ckb_Reforcement.Text = "Reforcement";
            this.ckb_Reforcement.UseVisualStyleBackColor = true;
            this.ckb_Reforcement.CheckedChanged += new System.EventHandler(this.Ckb_Reforcement_CheckedChanged);
            // 
            // ckb_Connection
            // 
            this.ckb_Connection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckb_Connection.AutoSize = true;
            this.ckb_Connection.Location = new System.Drawing.Point(115, 340);
            this.ckb_Connection.Name = "ckb_Connection";
            this.ckb_Connection.Size = new System.Drawing.Size(80, 17);
            this.ckb_Connection.TabIndex = 2;
            this.ckb_Connection.Text = "Connection";
            this.ckb_Connection.UseVisualStyleBackColor = true;
            this.ckb_Connection.CheckedChanged += new System.EventHandler(this.Ckb_Connection_CheckedChanged);
            // 
            // btn_OK
            // 
            this.btn_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_OK.Location = new System.Drawing.Point(193, 359);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 3;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(285, 359);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // FrmCopyReforcementNotMark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 394);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.ckb_Connection);
            this.Controls.Add(this.ckb_Reforcement);
            this.Controls.Add(this.lbx_Showelement);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCopyReforcementNotMark";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Copy Reforcement ";
            this.Load += new System.EventHandler(this.FrmCopyReforcementNotMark_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbx_Showelement;
        private System.Windows.Forms.CheckBox ckb_Reforcement;
        private System.Windows.Forms.CheckBox ckb_Connection;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
    }
}