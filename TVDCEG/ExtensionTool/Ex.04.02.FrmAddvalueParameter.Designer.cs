namespace TVDCEG.ExtensionTool
{ 
    partial class FrmAddvalueParameter
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
            this.lb_view = new System.Windows.Forms.ListBox();
            this.cbb_Parameter = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lb_view
            // 
            this.lb_view.FormattingEnabled = true;
            this.lb_view.Location = new System.Drawing.Point(12, 12);
            this.lb_view.Name = "lb_view";
            this.lb_view.Size = new System.Drawing.Size(381, 368);
            this.lb_view.TabIndex = 0;
            // 
            // cbb_Parameter
            // 
            this.cbb_Parameter.FormattingEnabled = true;
            this.cbb_Parameter.Location = new System.Drawing.Point(12, 411);
            this.cbb_Parameter.Name = "cbb_Parameter";
            this.cbb_Parameter.Size = new System.Drawing.Size(252, 21);
            this.cbb_Parameter.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(293, 412);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 2;
            // 
            // FrmAddvalueParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 483);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbb_Parameter);
            this.Controls.Add(this.lb_view);
            this.Name = "FrmAddvalueParameter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmAddvalueParameter";
            this.Load += new System.EventHandler(this.FrmAddvalueParameter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lb_view;
        private System.Windows.Forms.ComboBox cbb_Parameter;
        private System.Windows.Forms.TextBox textBox1;
    }
}