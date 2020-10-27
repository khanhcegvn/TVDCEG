namespace TVDCEG
{
    partial class FrmCopyConn_Dt_To_Dt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCopyConn_Dt_To_Dt));
            this.radioButton_flangeRight = new System.Windows.Forms.RadioButton();
            this.radioButton_flangeLeft = new System.Windows.Forms.RadioButton();
            this.btn_coon_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioButton_flangeRight
            // 
            this.radioButton_flangeRight.AutoSize = true;
            this.radioButton_flangeRight.Location = new System.Drawing.Point(173, 30);
            this.radioButton_flangeRight.Name = "radioButton_flangeRight";
            this.radioButton_flangeRight.Size = new System.Drawing.Size(85, 17);
            this.radioButton_flangeRight.TabIndex = 0;
            this.radioButton_flangeRight.TabStop = true;
            this.radioButton_flangeRight.Text = "Flange Right";
            this.radioButton_flangeRight.UseVisualStyleBackColor = true;
            this.radioButton_flangeRight.CheckedChanged += new System.EventHandler(this.RadioButton_flangeRight_CheckedChanged);
            // 
            // radioButton_flangeLeft
            // 
            this.radioButton_flangeLeft.AutoSize = true;
            this.radioButton_flangeLeft.Location = new System.Drawing.Point(13, 30);
            this.radioButton_flangeLeft.Name = "radioButton_flangeLeft";
            this.radioButton_flangeLeft.Size = new System.Drawing.Size(78, 17);
            this.radioButton_flangeLeft.TabIndex = 1;
            this.radioButton_flangeLeft.TabStop = true;
            this.radioButton_flangeLeft.Text = "Flange Left";
            this.radioButton_flangeLeft.UseVisualStyleBackColor = true;
            this.radioButton_flangeLeft.CheckedChanged += new System.EventHandler(this.RadioButton_flangeLeft_CheckedChanged);
            // 
            // btn_coon_ok
            // 
            this.btn_coon_ok.Location = new System.Drawing.Point(173, 63);
            this.btn_coon_ok.Name = "btn_coon_ok";
            this.btn_coon_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_coon_ok.TabIndex = 2;
            this.btn_coon_ok.Text = "OK";
            this.btn_coon_ok.UseVisualStyleBackColor = true;
            this.btn_coon_ok.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // FrmCopyConn_Dt_To_Dt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 98);
            this.Controls.Add(this.btn_coon_ok);
            this.Controls.Add(this.radioButton_flangeLeft);
            this.Controls.Add(this.radioButton_flangeRight);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCopyConn_Dt_To_Dt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Iven EXT: CopyConn_Dt_To_Dt";
            this.Load += new System.EventHandler(this.FrmCopyConn_Dt_To_Dt_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton_flangeRight;
        private System.Windows.Forms.RadioButton radioButton_flangeLeft;
        private System.Windows.Forms.Button btn_coon_ok;
    }
}