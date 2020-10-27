namespace TVDCEG
{
    partial class Frmchangeworkplane
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
            this.lbxplane = new System.Windows.Forms.ListBox();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxplane
            // 
            this.lbxplane.FormattingEnabled = true;
            this.lbxplane.Location = new System.Drawing.Point(12, 56);
            this.lbxplane.Name = "lbxplane";
            this.lbxplane.Size = new System.Drawing.Size(402, 342);
            this.lbxplane.TabIndex = 0;
            this.lbxplane.SelectedIndexChanged += new System.EventHandler(this.Lbxplane_SelectedIndexChanged);
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(12, 23);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(300, 20);
            this.textBox_Search.TabIndex = 1;
            this.textBox_Search.TextChanged += new System.EventHandler(this.TextBox_Search_TextChanged);
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(465, 395);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 2;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // Frmchangeworkplane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 430);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.textBox_Search);
            this.Controls.Add(this.lbxplane);
            this.Name = "Frmchangeworkplane";
            this.Text = "Frm";
            this.Load += new System.EventHandler(this.Frm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxplane;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.Button btn_OK;
    }
}