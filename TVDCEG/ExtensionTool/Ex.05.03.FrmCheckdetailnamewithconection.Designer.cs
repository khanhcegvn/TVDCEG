namespace TVDCEG.ExtensionTool
{
    partial class FrmCheckdetailnamewithconection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCheckdetailnamewithconection));
            this.lb_view = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lb_view
            // 
            this.lb_view.FormattingEnabled = true;
            this.lb_view.Location = new System.Drawing.Point(12, 10);
            this.lb_view.Name = "lb_view";
            this.lb_view.Size = new System.Drawing.Size(353, 394);
            this.lb_view.TabIndex = 0;
            // 
            // FrmCheckdetailnamewithconection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 450);
            this.Controls.Add(this.lb_view);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCheckdetailnamewithconection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Check detail name with conection";
            this.Load += new System.EventHandler(this.FrmCheckdetailnamewithconection_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_view;
    }
}